namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// This class converts text into a series of tokens, based on the regex patterns of each token definition.
    /// </summary>
    public class Tokenizer<T> : ITokenizer<T> where T : Enum {

        private readonly T invalidToken;
        private readonly T eolToken;
        private readonly T eofToken;

        private readonly List<TokenDefinition<T>> tokenDefinitions = new List<TokenDefinition<T>>();

        public IReadOnlyCollection<TokenDefinition<T>> TokenDefinitions => tokenDefinitions.AsReadOnly();


        public Tokenizer(T invalidToken, T eolToken, T eofToken, bool loadTokenDefinitions = true) {
            this.invalidToken = invalidToken;
            this.eolToken = eolToken;
            this.eofToken = eofToken;

            if (invalidToken.Equals(eolToken) || invalidToken.Equals(eofToken) || eolToken.Equals(eofToken))
                throw new ArgumentException($"{nameof(Tokenizer<T>)} must have unique reserved tokens (invalid, eol, eof)!");

            if (loadTokenDefinitions)
                LoadTokenDefinitions();
        }

        public void Add(TokenDefinition<T> tokenDefinition) {
            if (tokenDefinition == null)
                throw new ArgumentNullException(nameof(tokenDefinition));

            if (tokenDefinition.TokenType.Equals(invalidToken) || tokenDefinition.TokenType.Equals(eolToken) || tokenDefinition.TokenType.Equals(eofToken))
                throw new ArgumentException($"{nameof(Tokenizer<T>)} can't register a reserved token (invalid, eol, eof): {typeof(T).Name}.{tokenDefinition.TokenType}", nameof(tokenDefinition));

            if (tokenDefinitions.Contains(tokenDefinition))
                throw new ArgumentException($"{nameof(Tokenizer<T>)} can't register duplicate token definitions: {typeof(T).Name}.{tokenDefinition.TokenType} '{tokenDefinition.Regex}'", nameof(tokenDefinition));

            tokenDefinitions.Add(tokenDefinition);
        }

        public void Remove(TokenDefinition<T> tokenDefinition) {
            if (tokenDefinition == null)
                throw new ArgumentNullException(nameof(tokenDefinition));
            tokenDefinitions.Remove(tokenDefinition);
        }

        public void Clear() {
            tokenDefinitions.Clear();
        }

        /// <summary>
        /// Clears and loads all <see cref="TokenDef"/> attributes from the provided enum type.
        /// </summary>
        public void LoadTokenDefinitions() {
            Clear();

            foreach (FieldInfo member in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)) {
                foreach (TokenDef tokenDef in member.GetCustomAttributes<TokenDef>()) {
                    T tokenType = (T) member.GetValue(typeof(T));

                    Add(new TokenDefinition<T>(tokenType, tokenDef.Regex, tokenDef.Template, tokenDef.Precedence, tokenDef.HasVariableValue, tokenDef.RemoveRegex, tokenDef.RetargetToGroup, tokenDef.IgnoreCase));
                }
            }

        }


        public IEnumerable<Token<T>> Tokenize(IEnumerable<string> input) {
            int lineNumber = 1;

            foreach (string line in input) {

                foreach (Token<T> token in Tokenize(line, lineNumber))
                    yield return token;

                lineNumber++;
            }

            yield return new Token<T>(eofToken, lineNumber - 1);
        }

        public IEnumerable<Token<T>> Tokenize(string input, int lineNumber) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            IEnumerable<Token<T>> tokenMatches = FindAllTokenMatches(input, lineNumber);

            IEnumerable<IGrouping<int, Token<T>>> groupedByStartIndex = tokenMatches.GroupBy(x => x.StartIndex).OrderBy(x => x.Key);

            Token<T> lastMatch = null;

            foreach (IGrouping<int, Token<T>> grouping in groupedByStartIndex) {
                Token<T> token = grouping.OrderBy(x => x.Precedence).First();

                if (lastMatch != null) {  // Find invalid tokens.
                    int length = token.StartIndex - lastMatch.EndIndex;

                    if (length > 0) {
                        string invalidValue = input.Substring(lastMatch.EndIndex, length);
                        if (!string.IsNullOrWhiteSpace(invalidValue)) {
                            yield return new Token<T>(invalidToken, invalidValue, lastMatch.EndIndex, token.StartIndex, lineNumber);
                            lastMatch = token;
                            continue;
                        }

                    }
                } else {

                    // Find invalid tokens.
                    string invalidValue = input.Substring(0, token.StartIndex);
                    if (!string.IsNullOrWhiteSpace(invalidValue)) {
                        yield return new Token<T>(invalidToken, invalidValue, 0, token.StartIndex, lineNumber);

                        lastMatch = token;
                        continue;
                    }
                }

                if (lastMatch != null && token.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return token;

                lastMatch = token;
            }

            int eolIndex = lastMatch?.EndIndex ?? 0;
            yield return new Token<T>(eolToken, eolIndex, lineNumber);
        }

        /// <summary>
        /// Returns an IEnumerable of tokens for all matches based on the provided <paramref name="input"/> string.
        /// </summary>
        public IEnumerable<Token<T>> FindAllTokenMatches(string input, int lineNumber) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return tokenDefinitions.SelectMany(definition => definition.FindMatches(input, lineNumber));
        }

    }

}