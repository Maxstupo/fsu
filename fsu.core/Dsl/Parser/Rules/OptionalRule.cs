namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Utility;
    using System;

    public class OptionalRule<T> : Rule<T> where T : Enum {

        public object DefaultValue { get; }

        /// <summary>
        /// Defines a grammer rule where the current <see cref="Token{T}"/> type can be equal to the provided <paramref name="tokenType"/>, if equal and the provided <paramref name="pattern"/> isn't null, the <paramref name="pattern"/> must match the current <see cref="Token{T}"/> value. 
        /// </summary>
        /// <param name="tokenType">The token type that might match.</param>
        /// <param name="pattern">A regex that must match the token value for this rule to pass when the <paramref name="tokenType"/> matches, set null to disable.</param>
        public OptionalRule(T tokenType, object defaultValue = null, string pattern = null) : base(tokenType, pattern) {
            DefaultValue = defaultValue;
        }

        public OptionalRule(object defaultValue, params T[] tokenTypes) : base(tokenTypes) {
            DefaultValue = defaultValue;
        }

        public override bool Eval(ref TokenStack<T> stack, ref RuleData data) {
            Token<T> token = stack.Next();

            // TEMP
//#if DEBUG
//            new ColorConsole().WriteLine($"  - Checking {GetType().Name.Replace("`1", string.Empty)}: '{Pattern}' (&-a;{string.Join(", ", TokenTypes)}&-^;) => '&-e;{token.Value}&-^;' (&-a;{token.TokenType}&-^;)");
//#endif
            // Token type doesn't match, token isn't the one we are looking for... Revert stack, and return true.
            if (!IsTokenTypeMatch(token)) {
                stack.Prev();
                UpdateData(ref data, token, false);

                //TEMP
//#if DEBUG
//                new ColorConsole().WriteLine("    - Optional token missing, reverting...");
//#endif
                return true;
            }

            // Token with correct type exists, check if pattern matches.
            bool isMatch = IsPatternMatch(token);

            UpdateData(ref data, token, isMatch);

            return isMatch;
        }


        /// <summary>
        /// Adds the translated token value into the provided <paramref name="ruleData"/> object if the rule was a match, else the  default value of this rule will be added.
        /// </summary>
        protected override void UpdateData(ref RuleData ruleData, Token<T> token, bool isMatch) {
            ruleData.Add(isMatch ? GetValue(token) : DefaultValue);
        }

    }

}