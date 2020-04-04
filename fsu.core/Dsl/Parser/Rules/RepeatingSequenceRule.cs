using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Dsl.Parser.Rules {
    public class RepeatingSequenceRule<T> : Rule<T>, IEnumerable<Rule<T>> where T : Enum {

        private readonly List<Rule<T>> rules = new List<Rule<T>>();

        /// <summary>
        /// Defines a rule that can have a set of tokens repeat when they seperate by the provided <paramref name="seperatorTokens"/>.
        /// </summary>
        /// <param name="seperatorTokens">An array of tokens that denote the seperation of each rule sequence.</param>
        public RepeatingSequenceRule(params T[] seperatorTokens) : base(seperatorTokens) { }


        public RepeatingSequenceRule<T> Add(Rule<T> rule) {
            if (rule is RepeatingSequenceRule<T>)
                throw new ArgumentException($"Can't have nested {nameof(RepeatingSequenceRule<T>)}s", nameof(rule));
            rules.Add(rule);
            return this;
        }

        public override bool Check(ref TokenStack<T> stack) {
            new ColorConsole().WriteLine($"  - Checking {GetType().Name.Replace("`1", string.Empty)}");

            foreach (Rule<T> rule in rules) {
                if (!rule.Check(ref stack))
                    return false;
            }

            if (IsTokenTypeMatch(stack.Next())) {
                return Check(ref stack);
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
