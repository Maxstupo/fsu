using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Dsl.Parser {

    public class Grammer<T, V> : IEnumerable<Rule<T>> where T : Enum where V : class {

        public T TriggerTokenToken { get; }

        public string TriggerTokenValuePattern { get; }

        /// <summary>If true, remove all null entries from the <see cref="RuleData"/>. Useful for optional rules.</summary>
        public bool CleanRuleData { get; set; } 

        /// <summary>If true, include the trigger token when evaluating the rules.</summary>
        public bool IncludeTriggerToken { get; set; } = false;

        public List<Rule<T>> Rules { get; } = new List<Rule<T>>();

        /// <summary>
        /// A function that is called when this Grammer evaluated sucessfully. 
        /// Provides access to <see cref="RuleData"/> containing values from each rule, the returned value will be added to the parser result list.
        /// <br/>The Function can return null or the Construct delegate can be null doing so will cause the method and or result to be ignored.
        /// </summary>
        public Func<RuleData, V> Construct;

        //TODO: Support multiple trigger token types?
        public Grammer(T triggerTokenType, string triggerTokenValuePattern = null, bool cleanRuleData = true) {
            TriggerTokenToken = triggerTokenType;
            TriggerTokenValuePattern = triggerTokenValuePattern;
            CleanRuleData = cleanRuleData;
        }

        public bool Eval(ref TokenStack<T> stack, out V result) {
            Token<T> token = stack.Peek();

            if (IncludeTriggerToken)
                stack.Prev();

            //TEMP
            token.WriteLine(new ColorConsole());

            RuleData data = new RuleData();

            foreach (Rule<T> rule in Rules) {
                if (!rule.Eval(ref stack, ref data)) {
                    result = null;
                    return false;
                }
            }

            if (CleanRuleData)
                data.RemoveAll(x => x == null);

            result = Construct?.Invoke(data) ?? null;
            return true;
        }


        /// <summary>
        /// Checks if the provided <paramref name="token"/> matches the TriggerToken and optionally the TriggerPattern.
        /// </summary>
        public virtual bool IsMatch(Token<T> token) {
            return token.TokenType.Equals(TriggerTokenToken) && (TriggerTokenValuePattern == null || Regex.IsMatch(token.Value, TriggerTokenValuePattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
        }

        public void Add(Rule<T> rule) {
            Rules.Add(rule);
        }

        public IEnumerator<Rule<T>> GetEnumerator() {
            return Rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

}
