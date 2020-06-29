namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Maxstupo.Fsu.Core.Utility;

    /// <summary>
    /// This class converts text into a series of tokens, based on the regex patterns of each token definition.
    /// </summary>
    public class Tokenizer<T> : ITokenizer<T> where T : Enum {
    
        private readonly T invalidToken;
        private readonly T eolToken;
        private readonly T eofToken;

        private readonly List<TokenDefinition<T>> tokenDefinitions = new List<TokenDefinition<T>>();

        public Tokenizer(T invalidToken, T eolToken, T eofToken, bool loadTokenDefinitions = true) {
            this.invalidToken = invalidToken;
            this.eolToken = eolToken;
            this.eofToken = eofToken;

            if (loadTokenDefinitions)
                LoadTokenDefinitions();
        }

        /// <summary>
        /// Clears all definitions and then loads the token definitions from the provided enum type. Use the <see cref="TokenDef"/> attribute.
        /// </summary>
        public void LoadTokenDefinitions() {
            Clear();

            foreach (FieldInfo member in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)) {

                foreach (TokenDef tokenDef in member.GetCustomAttributes<TokenDef>()) {
                    T tokenType = (T) member.GetValue(typeof(T));

                    Add(new TokenDefinition<T>(tokenType, tokenDef.Regex, tokenDef.Template, tokenDef.Precedence, tokenDef.HasVariableValue, tokenDef.RemoveRegex,tokenDef.RetargetToGroup));
                }
            }


        }

        /// <summary>
        /// Registers the specified <see cref="TokenDefinition{T}"/> with this <see cref="Tokenizer{T}"/>. Throws an <see cref="ArgumentException"/> if the token has already been registered or is a reserved token (invalid, eol, eof).
        /// </summary>
        /// <param name="definition"></param>
        /// <exception cref="ArgumentException">Throws if the token has already been registered or is a reserved token (invalid, eol, eof).</exception>
        public void Add(TokenDefinition<T> definition) {

            if (definition.TokenType.Equals(invalidToken) || definition.TokenType.Equals(eolToken) || definition.TokenType.Equals(eofToken))
                throw new ArgumentException($"{nameof(Tokenizer<T>)} can't register a reserved token (invalid, eol, eof): {typeof(T).Name}.{definition.TokenType}", nameof(definition));

            if (tokenDefinitions.Contains(definition))
                throw new ArgumentException($"{nameof(Tokenizer<T>)} can't register duplicate token definitions: {typeof(T).Name}.{definition.TokenType} '{definition.Regex}'", nameof(definition));

            //new ColorConsole().WriteLine($"Adding token definition: '&-e;{definition.Regex}&-^;' &-9;{definition.Precedence}&-^; (&-a;{definition.TokenType}&-^;)");

            tokenDefinitions.Add(definition);
        }

        /// <summary>
        /// Clears all registered token definitions.
        /// </summary>
        public void Clear() {
            tokenDefinitions.Clear();
        }

        /// <summary>
        /// Tokenizes an enumerable, treating each item in the enumerable as a seperate line.
        /// </summary>
        /// <returns>The tokenized representation of the provided input.</returns>
        public IEnumerable<Token<T>> Tokenize(IEnumerable<string> input) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int lineNumber = 1;

            foreach (string line in input) {

                foreach (Token<T> token in Tokenize(line, lineNumber))
                    yield return token;

                lineNumber++;
            }

            yield return new Token<T>(eofToken, lineNumber - 1);
        }

        /// <summary>
        /// Tokenizes a given string, treating it as a single line of text.
        /// </summary>
        /// <returns>The tokenized representation of the provided input.</returns>
        public IEnumerable<Token<T>> Tokenize(string input, int lineNumber) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            IEnumerable<Token<T>> tokenMatches = FindAllTokenMatches(input, lineNumber);

            IEnumerable<IGrouping<int, Token<T>>> groupedByStartIndex = tokenMatches.GroupBy(x => x.StartIndex).OrderBy(x => x.Key);

            Token<T> lastMatch = null;

            foreach (IGrouping<int, Token<T>> grouping in groupedByStartIndex) {
                Token<T> token = grouping.OrderBy(x => x.Precedence).First();

                //TODO: Improve error reporting code (cleanup).
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
        /// Returns an IEnumerable of tokens for all possible token matches for the provided <paramref name="input"/> string.
        /// </summary>
        public IEnumerable<Token<T>> FindAllTokenMatches(string input, int lineNumber) {
            return tokenDefinitions.SelectMany(definition => definition.FindMatches(input, lineNumber));
        }

    }

}