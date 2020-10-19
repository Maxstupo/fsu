namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Dsl.Lexer;

    /// <summary>
    /// A parser that converts a sequence of tokens into a list of objects, while ensuring correct grammar is adhered to with the token sequence.
    /// </summary>
    public class TokenParser<T, V> : ITokenParser<T, V> where T : Enum where V : class {

        private readonly ISet<Grammar<T, V>> grammars = new HashSet<Grammar<T, V>>();

        public event EventHandler<Grammar<T, V>> OnGrammarAdded;
        public event EventHandler<Grammar<T, V>> OnGrammarRemoved;
        public event EventHandler OnGrammarsCleared;

        public event EventHandler<Token<T>> OnTokenError;
        public event EventHandler<Token<T>> OnTokenParsing;

        private readonly T commentToken;
        private readonly T eolToken;
        private readonly T eofToken;
        private readonly T invalidToken;

        public TokenParser(T commentToken, T eolToken, T eofToken, T invalidToken) {
            this.commentToken = commentToken;
            this.eolToken = eolToken;
            this.eofToken = eofToken;
            this.invalidToken = invalidToken;
        }

        /// <summary>
        /// Clears all registered grammars, and notifies event handlers. 
        /// </summary>
        public void Clear() {
            grammars.Clear();
            OnGrammarsCleared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Adds the specified <paramref name="grammar"/> to this <see cref="TokenParser{T, V}"/>, and notifies event handlers.
        /// </summary>
        /// <returns>The providied <paramref name="grammar"/> object.</returns>
        public Grammar<T, V> Add(Grammar<T, V> grammar) {
            if (grammar == null)
                throw new ArgumentNullException(nameof(grammar));

            if (grammars.Add(grammar))
                OnGrammarAdded?.Invoke(this, grammar);

            return grammar;
        }

        /// <summary>
        /// Removes the specified <paramref name="grammar"/> from this <see cref="TokenParser{T, V}"/>, and notifies event handlers.
        /// </summary>
        public void Remove(Grammar<T, V> grammar) {
            if (grammar == null)
                throw new ArgumentNullException(nameof(grammar));

            if (grammars.Remove(grammar))
                OnGrammarRemoved?.Invoke(this, grammar);
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

                    List<Grammar<T, V>> matchedGrammars = grammars.Where(x => x.IsMatch(token)).ToList();

                    if (matchedGrammars.Count == 0) // No grammars exist for the token.
                        hasError = true;

                    for (int i = 0; i < matchedGrammars.Count; i++) {
                        Grammar<T, V> grammar = matchedGrammars[i];

                        stack.Mark();

                        OnTokenParsing?.Invoke(this, stack.Peek());

                        if (grammar.Eval(ref stack, out V result)) {
                            if (result != null)
                                objects.Add(result);
                            break;
                        } else if (i == matchedGrammars.Count - 1) { // Last grammar available failed, indicate an error.
                            hasError = true;
                            token = stack.Peek(); // update token location for error reporting.
                        } else {
                            stack.Jump(); // Revert back for next grammar test.
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
