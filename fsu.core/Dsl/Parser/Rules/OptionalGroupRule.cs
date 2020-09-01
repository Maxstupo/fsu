namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Dsl.Lexer;


    public class OptionalGroupRule<T> : IRule<T> where T : Enum {
        private readonly object[] defaultValues;

        public List<IRule<T>> Rules { get; } = new List<IRule<T>>();

        public OptionalGroupRule(params object[] defaultValues) {
            this.defaultValues = defaultValues;
        }

        public bool Eval(ref TokenStack<T> stack, ref RuleData data) {

            int originalIndex = data.Count;

            int successCount = 0;

            stack.Mark();
            for (int i = 0; i < Rules.Count; i++) {
                IRule<T> rule = Rules[i];

                if (rule.Eval(ref stack, ref data))
                    successCount++;
            }

            //


            if (successCount == 0) {
               // data.RemoveRange(originalIndex, data.Count - originalIndex);
              //  data.AddRange(defaultValues);
                stack.Jump();

                return true;
            }
            stack.Unmark();


            //  
            return successCount == Rules.Count;
        }

    }

}