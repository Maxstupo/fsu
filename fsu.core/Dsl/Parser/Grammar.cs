namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
    using Maxstupo.Fsu.Core.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class Grammar<T, V> : IEnumerable<IRule<T>>, IEquatable<Grammar<T, V>> where T : Enum where V : class {

        public T[] TriggerTokens { get; }

        public string TriggerTokenValuePattern { get; }

        /// <summary>If true, remove all null entries from the <see cref="RuleData"/>. Useful for optional rules.</summary>
        public bool CleanRuleData { get; set; }

        /// <summary>If true, include the trigger token when evaluating the rules.</summary>
        public bool IncludeTriggerToken { get; set; } = false;

        public List<IRule<T>> Rules { get; } = new List<IRule<T>>();

        /// <summary>
        /// A function that is called when this Grammer evaluated sucessfully. 
        /// Provides access to <see cref="RuleData"/> containing values from each rule, the returned value will be added to the parser result list.
        /// <br/>The Function can return null or the Construct delegate can be null doing so will cause the method and or result to be ignored.
        /// </summary>
        public Func<RuleData, V> Construct;

        public Grammar(T triggerTokenType, string triggerTokenValuePattern = null, bool cleanRuleData = true) {
            TriggerTokens = new T[] { triggerTokenType };
            TriggerTokenValuePattern = triggerTokenValuePattern;
            CleanRuleData = cleanRuleData;
        }

        public Grammar(params T[] triggerTokenType) {
            TriggerTokens = triggerTokenType;
            TriggerTokenValuePattern = null;
            CleanRuleData = true;
        }

        public bool Eval(ref TokenStack<T> stack, out V result) {
            Token<T> token = stack.Peek();

            if (IncludeTriggerToken) 
                 stack.Prev();
                        
            RuleData data = new RuleData();

            foreach (Rule<T> rule in Rules) {
                if (!rule.Eval(ref stack, ref data)) {
                    result = null;
                    return false;
                }
            }

            if (CleanRuleData)
                data.RemoveAll(x => x == null);

            try {
                result = Construct?.Invoke(data) ?? null;
                return true;
            } catch (Exception) {
                result = null;
                return false;
            }
        }


        /// <summary>
        /// Checks if the provided <paramref name="token"/> matches the any of the trigger token types and optionally the value pattern.
        /// </summary>
        public virtual bool IsMatch(Token<T> token) {
            return TriggerTokens.Any(x => x.Equals(token.TokenType)) && (TriggerTokenValuePattern == null || Regex.IsMatch(token.Value, TriggerTokenValuePattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
        }

        public void Add(IRule<T> rule) {
            Rules.Add(rule);
        }

        public IEnumerator<IRule<T>> GetEnumerator() {
            return Rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override bool Equals(object obj) {
            return Equals(obj as Grammar<T, V>);
        }

        public bool Equals(Grammar<T, V> other) {
            return other != null &&
                   EqualityComparer<T[]>.Default.Equals(this.TriggerTokens, other.TriggerTokens) &&
                   this.TriggerTokenValuePattern == other.TriggerTokenValuePattern &&
                   this.CleanRuleData == other.CleanRuleData &&
                   this.IncludeTriggerToken == other.IncludeTriggerToken &&
                   EqualityComparer<List<IRule<T>>>.Default.Equals(this.Rules, other.Rules);
        }

        public override int GetHashCode() {
            int hashCode = 206486245;
            hashCode = hashCode * -1521134295 + EqualityComparer<T[]>.Default.GetHashCode(this.TriggerTokens);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.TriggerTokenValuePattern);
            hashCode = hashCode * -1521134295 + this.CleanRuleData.GetHashCode();
            hashCode = hashCode * -1521134295 + this.IncludeTriggerToken.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<IRule<T>>>.Default.GetHashCode(this.Rules);
            return hashCode;
        }

        public static bool operator ==(Grammar<T, V> left, Grammar<T, V> right) {
            return EqualityComparer<Grammar<T, V>>.Default.Equals(left, right);
        }

        public static bool operator !=(Grammar<T, V> left, Grammar<T, V> right) {
            return !(left == right);
        }

    }

}