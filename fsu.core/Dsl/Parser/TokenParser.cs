namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A parser that converts a sequence of tokens into a list of objects, while ensuring correct grammer is adhered to with the token sequence.
    /// </summary>
    public class TokenParser<T, V> : ITokenParser<T, V> where T : Enum where V : class {

        private readonly IConsole console;

        private readonly T commentToken;
        private readonly T eolToken;
        private readonly T eofToken;
        private readonly T invalidToken;

        private readonly ISet<Grammer<T, V>> grammers = new HashSet<Grammer<T, V>>();

        public TokenParser(IConsole console, T commentToken, T eolToken, T eofToken, T invalidToken) {
            this.console = console;
            this.commentToken = commentToken;
            this.eolToken = eolToken;
            this.eofToken = eofToken;
            this.invalidToken = invalidToken;
        }

        public void Clear() {
            grammers.Clear();
        }

        public Grammer<T, V> Add(Grammer<T, V> grammer) {
            if (grammer == null)
                throw new ArgumentNullException(nameof(grammer));
            console.WriteLine($"Adding grammer: '&-b;{grammer.TriggerTokenValuePattern}&-^;' (&-a;{string.Join(", ", grammer.TriggerTokenTokens)}&-^;) with {grammer.Rules.Count} rule(s)");
            grammers.Add(grammer);
            return grammer;
        }

        public void Remove(Grammer<T, V> grammer) {
            grammers.Remove(grammer);
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

                    // TODO: Should we allow multiple matching grammers?
                    List<Grammer<T, V>> matchedGrammers = grammers.Where(x => x.IsMatch(token)).ToList();

                    if (matchedGrammers.Count == 0) // No grammers exist for the token.
                        hasError = true;

                    for (int i = 0; i < matchedGrammers.Count; i++) {
                        Grammer<T, V> grammer = matchedGrammers[i];

                        stack.Mark();

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
                    console.WriteLine($"&-c;ERROR - Unexpected token: '{token.Value}' ({token.TokenType}) {token.Location}&-^;");
                    objects.Clear();
                    break;
                }

            }

            return objects;

        }

    }

}