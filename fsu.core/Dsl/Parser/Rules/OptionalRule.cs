using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Utility;
using System;

namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {
    public class OptionalRule<T> : Rule<T> where T : Enum {
        public OptionalRule(T tokenType, string pattern = null) : base(tokenType, pattern) {
        }

        public override bool Check(ref TokenStack<T> stack) {
            Token<T> token = stack.Next();

            new ColorConsole().WriteLine($"  - Checking {GetType().Name.Replace("`1", string.Empty)}: '{Pattern}' (&-a;{string.Join(", ", TokenTypes)}&-^;) => '&-e;{token.Value}&-^;' (&-a;{token.TokenType}&-^;)");

            if (!IsTokenTypeMatch(token)) {
                stack.Prev(); new ColorConsole().WriteLine("    - Optional token missing, reverting...");
                return true;
            }
            return IsPatternMatch(token);
        }
    }

}
