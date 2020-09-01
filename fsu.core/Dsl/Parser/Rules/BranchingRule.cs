namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Dsl.Lexer;

    /// <summary>
    /// A rule that will eval each branch made up of many rules until a branch doesn't fail. Similar to a selector (fallback) "node" in behaviour trees.
    /// This rule sets a RuleData item that represents the index of the branch that was successfully evaluated.
    /// </summary>
    public class BranchingRule<T> : IRule<T> where T : Enum {

        public List<List<Rule<T>>> Branches { get; } = new List<List<Rule<T>>>();

        private readonly bool noMatchFail;

        public BranchingRule(bool noMatchFail = false) {
            this.noMatchFail = noMatchFail;
        }

        public bool Eval(ref TokenStack<T> stack, ref RuleData data) {

            int originalIndex = data.Count;

            for (int i = 0; i < Branches.Count; i++) {
                List<Rule<T>> rules = Branches[i];
                stack.Mark();
                {
                    if (EvalRules(ref stack, ref data, rules)) {
                    //    data.Insert(originalIndex, i);
                        stack.Unmark();
                        return true;
                    }
                }
                stack.Jump();
              //  if (noMatchFail)
                //    data.RemoveRange(originalIndex, data.Count - originalIndex);
            }
            stack.Next();
            return noMatchFail;
        }

        private static bool EvalRules(ref TokenStack<T> stack, ref RuleData data, List<Rule<T>> rules) {
            foreach (Rule<T> rule in rules) {
                if (!rule.Eval(ref stack, ref data))
                    return false;
            }
            return true;
        }

    }

}