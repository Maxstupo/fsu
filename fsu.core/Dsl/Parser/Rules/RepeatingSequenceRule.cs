namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class RepeatingSequenceRule<T> : Rule<T>, IEnumerable<Rule<T>> where T : Enum {

        private readonly List<Rule<T>> rules = new List<Rule<T>>();
        private readonly bool includeSeperator;

        /// <summary>
        /// Defines a rule that can have a set of tokens repeat when they seperate by the provided <paramref name="seperatorTokens"/>.
        /// </summary>
        /// <param name="includeSeperator">If true the seperator value will be included in the rule data.</param>
        /// <param name="seperatorTokens">An array of tokens that denote the seperation of each rule sequence.</param>
        public RepeatingSequenceRule(bool includeSeperator, params T[] seperatorTokens) : base(seperatorTokens) {
            this.includeSeperator = includeSeperator;
        }


        public RepeatingSequenceRule<T> Add(Rule<T> rule) {
            if (rule is RepeatingSequenceRule<T>)
                throw new ArgumentException($"Can't have nested {nameof(RepeatingSequenceRule<T>)}s", nameof(rule));
            rules.Add(rule);
            return this;
        }

        public override bool Eval(ref TokenStack<T> stack, ref RuleData data) {
            //TEMP

//            new ColorConsole().WriteLine(Level.Fine, $"  - Checking {GetType().Name.Replace("`1", string.Empty)}");

            foreach (Rule<T> rule in rules) {
                if (!rule.Eval(ref stack, ref data))
                    return false;
            }

            if (IsTokenTypeMatch(stack.Next())) {
                UpdateData(ref data, stack.Peek(), includeSeperator);
                return Eval(ref stack, ref data);
            } else {
                stack.Prev();
            }

            return true;
        }

        public IEnumerator<Rule<T>> GetEnumerator() {
            return rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
 
    }

}