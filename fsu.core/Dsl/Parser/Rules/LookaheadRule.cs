using Maxstupo.Fsu.Core.Dsl.Lexer;
using System;

namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {

    public class LookaheadRule<T> : Rule<T> where T : Enum {

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// <br/>
        /// Rule will return to original token, no rule data will be generated.
        /// </summary>
        public LookaheadRule(params T[] tokenTypes) : base(tokenTypes) {
        }

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// <br/>
        /// Rule will return to original token, no rule data will be generated.
        /// </summary>
        public LookaheadRule(T tokenType, string pattern = null) : base(tokenType, pattern) {
        }

        public override bool Eval(ref TokenStack<T> tokenStack, ref RuleData data) {
            bool isMatch = base.Eval(ref tokenStack, ref data);
            if (isMatch)
                tokenStack.Prev();
            return isMatch;
        }

        /// <summary>Disabled.<br/><br/>Dont generate any rule data.</summary>
        protected override void UpdateData(ref RuleData data, Token<T> token, bool isMatch) { }

    }

}
