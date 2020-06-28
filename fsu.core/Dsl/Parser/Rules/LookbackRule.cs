namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using System;

    public class LookbackRule<T> : Rule<T> where T : Enum {

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// <br/>
        /// Rule will return to original token, no rule data will be generated.
        /// </summary>
        public LookbackRule(params T[] tokenTypes) : base(tokenTypes) {
        }

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// <br/>
        /// Rule will return to original token, no rule data will be generated.
        /// </summary>
        public LookbackRule(T tokenType, string pattern = null) : base(tokenType, pattern) {
        }

        public override bool Eval(ref TokenStack<T> tokenStack, ref RuleData data) {
            tokenStack.Prev();
            bool isMatch = base.Eval(ref tokenStack, ref data);
            return isMatch;
        }

        /// <summary>Disabled.<br/><br/>Dont generate any rule data.</summary>
        protected override void UpdateData(ref RuleData data, Token<T> token, bool isMatch) { }

    }

}