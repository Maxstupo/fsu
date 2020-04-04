using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {
    public class Rule<T> where T : Enum {

        public T[] TokenTypes { get; }

        public string Pattern { get; }

        //  public Func<string, object> ValueConverter { get; set; }

        /// <summary>
        /// Defines a grammer rule where the current <see cref="Token{T}"/> type must equal the provided <paramref name="tokenType"/>, and optionally the value must match the provided <paramref name="pattern"/>. 
        /// </summary>
        /// <param name="tokenType">The token type that must match.</param>
        /// <param name="pattern">A regex that must match the token value for this rule to pass, set null to disable.</param>
        public Rule(T tokenType, string pattern = null) {
            TokenTypes = new T[] { tokenType };
            Pattern = pattern;
        }

        /// <summary>
        /// Defines a grammer rule where the current <see cref="Token{T}"/> type must be one of the provided <paramref name="tokenTypes"/>.
        /// </summary>
        /// <param name="tokenTypes"></param>
        public Rule(params T[] tokenTypes) {
            TokenTypes = tokenTypes;
            Pattern = null;
        }

        public virtual bool Check(ref TokenStack<T> stack) {

            Token<T> token = stack.Next();

            new ColorConsole().WriteLine($"  - Checking {GetType().Name.Replace("`1", string.Empty)}: '{Pattern}' (&-a;{string.Join(", ", TokenTypes)}&-^;) => '&-e;{token.Value}&-^;' (&-a;{token.TokenType}&-^;)");

            return IsTokenTypeMatch(token) && IsPatternMatch(token);
        }

        protected virtual bool IsTokenTypeMatch(Token<T> token) {
            return TokenTypes.Any(x => x.Equals(token.TokenType));
        }

        protected virtual bool IsPatternMatch(Token<T> token) {
            return Pattern == null || Regex.IsMatch(token.Value, Pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

    }

}
