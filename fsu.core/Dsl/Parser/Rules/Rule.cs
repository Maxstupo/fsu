using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {
    public class Rule<T> where T : Enum {

        public T[] TokenTypes { get; }

        public string Pattern { get; }

        /// <summary>
        /// Converts the token value for this rule into an object usable within the <see cref="Grammer{T, V}.Construct"/> delegate. By default, no conversion takes place.
        /// </summary>
        public Func<string, object> ValueConverter { get; set; }

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
        public Rule(params T[] tokenTypes) {
            TokenTypes = tokenTypes;
            Pattern = null;
        }

        /// <summary>
        /// Evaluates the provided <paramref name="tokenStack"/> and returns true if the next token in the stack matches any of the <see cref="TokenTypes"/> and the token value matches the optional <see cref="Pattern"/>.
        /// <br/>Note: Increments the provided <paramref name="tokenStack"/>. See <see cref="TokenStack{T}.Next"/>
        /// </summary>
        public virtual bool Eval(ref TokenStack<T> tokenStack, ref RuleData data) {

            Token<T> token = tokenStack.Next();

            // TEMP
            new ColorConsole().WriteLine($"  - Checking {GetType().Name.Replace("`1", string.Empty)}: '{Pattern}' (&-a;{string.Join(", ", TokenTypes)}&-^;) => '&-e;{token.Value}&-^;' (&-a;{token.TokenType}&-^;)");

            bool isMatch = IsTokenTypeMatch(token) && IsPatternMatch(token);

            UpdateData(ref data, token, isMatch);
            return isMatch;
        }

        /// <summary>
        /// Adds the translated token value into the provided <paramref name="ruleData"/> object if the rule was a match.
        /// </summary>
        protected virtual void UpdateData(ref RuleData ruleData, Token<T> token, bool isMatch) {
            if (isMatch)
                ruleData.Add(GetValue(token));
        }

        /// <summary>
        /// Returns the value of the provided <paramref name="token"/> converted into the correct format for <see cref="Grammer{T, V}.Construct"/>.
        /// </summary>
        protected virtual object GetValue(Token<T> token) {
            if (ValueConverter == null)
                return token.Value;
            return ValueConverter(token.Value);
        }

        /// <summary>
        /// Checks if the provided <see cref="Token{T}"/> matches any of the rule token types.
        /// </summary>
        protected virtual bool IsTokenTypeMatch(Token<T> token) {
            return TokenTypes.Any(x => x.Equals(token.TokenType));
        }

        /// <summary>
        /// Checks if the provided <see cref="Token{T}"/> value matches the rule regex pattern, if the pattern is null return true.
        /// </summary>
        protected virtual bool IsPatternMatch(Token<T> token) {
            return Pattern == null || Regex.IsMatch(token.Value, Pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

    }

}
