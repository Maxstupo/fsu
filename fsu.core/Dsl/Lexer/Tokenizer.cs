using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    public class Tokenizer<T> : ITokenizer<T> where T : Enum {

        public T InvalidToken { get; }

        public T EndOfLineToken { get; }

        public T EndOfFileToken { get; }


        private readonly List<TokenDefinition<T>> tokenDefinitions = new List<TokenDefinition<T>>();


        public Tokenizer(T invalidToken, T eolToken, T eofToken, bool loadTokenDefinitions = true) {
            InvalidToken = invalidToken;
            EndOfLineToken = eolToken;
            EndOfFileToken = eofToken;

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

                    AddDef(new TokenDefinition<T>(tokenType, tokenDef.Regex, tokenDef.Template,tokenDef.Precedence, tokenDef.HasVariableValue));
                }
            }

            
        }


        public void AddDef(TokenDefinition<T> definition) {

            if (definition.TokenType.Equals(InvalidToken) || definition.TokenType.Equals(EndOfLineToken) || definition.TokenType.Equals(EndOfFileToken))
                throw new ArgumentException($"{nameof(Tokenizer<T>)} can't register a reserved token (invalid, eol, eof): {typeof(T).Name}.{definition.TokenType}", nameof(definition));

            tokenDefinitions.Add(definition);

            ColorConsole.WriteLine($"{nameof(TokenDefinition<T>)}: &-e;{definition.TokenType}&-^; ({definition.Precedence}) -> &-b;{definition.Pattern}&-^;");
        }

        public void Clear() {
            tokenDefinitions.Clear();
        }


        public IEnumerable<Token<T>> Parse(IEnumerable<string> input) {

            int lineNumber = 1;

            foreach (string line in input) {

                foreach (Token<T> token in Parse(line, lineNumber))
                    yield return token;

                lineNumber++;
            }

            yield return new Token<T>(EndOfFileToken, lineNumber - 1);
        }

        public IEnumerable<Token<T>> Parse(string input, int lineNumber) {
            IEnumerable<Token<T>> tokenMatches = FindAllTokenMatches(input, lineNumber);

            IEnumerable<IGrouping<int, Token<T>>> groupedByStartIndex = tokenMatches.GroupBy(x => x.StartIndex).OrderBy(x => x.Key);

            Token<T> lastMatch = null;

            foreach (IGrouping<int, Token<T>> grouping in groupedByStartIndex) {
                Token<T> token = grouping.OrderBy(x => x.Precedence).First();

                //TODO: Fix error reporting
                if (lastMatch != null) {  // Find invalid tokens.
                    int length = token.StartIndex - lastMatch.EndIndex;

                    if (length > 0) {
                        string invalidValue = input.Substring(lastMatch.EndIndex, length);
                        if (!string.IsNullOrWhiteSpace(invalidValue)) {
                            yield return new Token<T>(InvalidToken, invalidValue, lastMatch.EndIndex, token.StartIndex, lineNumber);
                            lastMatch = token;
                            continue;
                        }

                    }
                } else {

                    // Find invalid tokens.
                    string invalidValue = input.Substring(0, token.StartIndex);
                    if (!string.IsNullOrWhiteSpace(invalidValue)) {
                        yield return new Token<T>(InvalidToken, invalidValue, 0, token.StartIndex, lineNumber);

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
            yield return new Token<T>(EndOfLineToken, eolIndex, lineNumber);
        }


        public IEnumerable<Token<T>> FindAllTokenMatches(string input, int lineNumber) {
            return tokenDefinitions.SelectMany(definition => definition.FindMatches(input, lineNumber));
        }

    }

}
