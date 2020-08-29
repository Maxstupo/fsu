namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A parser that converts a sequence of tokens into a list of objects, while ensuring correct grammer is adhered to with the token sequence.
    /// </summary>
    public class TokenParser<T, V> : ITokenParser<T, V> where T : Enum where V : class {

        private readonly T commentToken;
        private readonly T eolToken;
        private readonly T eofToken;
        private readonly T invalidToken;

        private readonly ISet<Grammar<T, V>> grammers = new HashSet<Grammar<T, V>>();

        public event EventHandler<Grammar<T, V>> OnGrammerAdded;
        public event EventHandler<Grammar<T, V>> OnGrammerRemoved;
        public event EventHandler OnGrammersCleared;

        public event EventHandler<Token<T>> OnTokenError;
        public event EventHandler<Token<T>> OnTokenParsing;

        public TokenParser(T commentToken, T eolToken, T eofToken, T invalidToken) {
            this.commentToken = commentToken;
            this.eolToken = eolToken;
            this.eofToken = eofToken;
            this.invalidToken = invalidToken;
        }

        public void Clear() {
            OnGrammersCleared?.Invoke(this, EventArgs.Empty);
            grammers.Clear();
        }

        public Grammar<T, V> Add(Grammar<T, V> grammer) {
            if (grammer == null)
                throw new ArgumentNullException(nameof(grammer));

            grammers.Add(grammer);

            OnGrammerAdded?.Invoke(this, grammer);


            return grammer;
        }

        public void Remove(Grammar<T, V> grammer) {
            grammers.Remove(grammer);
            OnGrammerRemoved?.Invoke(this, grammer);
        }

        public List<V> Parse(IEnumerable<Token<T>> tokens) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            List<V> objects = new List<V>();

            TokenStack<T> stack = new TokenStack<T>(tokens);

            bool inComment = false;

            while (stack.HasNext) {

                Token<T> token = stack.Next();
                bool hasError = false;

                if (token.TokenType.Equals(commentToken)) {
                    inComment = true;

                } else if (token.TokenType.Equals(eolToken)) {
                    inComment = false;

                } else if (token.TokenType.Equals(eofToken)) {


                } else if (token.TokenType.Equals(invalidToken)) {
                    hasError = true;

                } else if (!inComment) {

                    List<Grammar<T, V>> matchedGrammers = grammers.Where(x => x.IsMatch(token)).ToList();

                    if (matchedGrammers.Count == 0) // No grammers exist for the token.
                        hasError = true;

                    for (int i = 0; i < matchedGrammers.Count; i++) {
                        Grammar<T, V> grammer = matchedGrammers[i];

                        stack.Mark();

                        OnTokenParsing?.Invoke(this, stack.Peek());

                        if (grammer.Eval(ref stack, out V result)) {
                            if (result != null)
                                objects.Add(result);
                            break;
                        } else if (i == matchedGrammers.Count - 1) { // Last grammer available failed, indicate an error.
                            hasError = true;
                            token = stack.Peek(); // update token location for error reporting.
                        } else {
                            stack.Jump(); // Revert back for next grammer test.
                        }
                    }

                }

                if (hasError) {
                    OnTokenError?.Invoke(this, token);
                    objects.Clear();
                    break;
                }

            }

            return objects;

        }

    }

}