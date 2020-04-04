using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Dsl.Parser {
    public class Grammer<T, V> : IEnumerable<Rule<T>> where T : Enum where V : class {

        public T TriggerToken { get; }

        public string TriggerPattern { get; }

        public List<Rule<T>> Rules { get; } = new List<Rule<T>>();

        public Func<List<object>, V> Construct;

        public Grammer(T trigger, string triggerPattern = null) {
            TriggerToken = trigger;
            TriggerPattern = triggerPattern;
        }

        public bool Run(ref TokenStack<T> stack, out V result) {

            Token<T> token = stack.Peek();

            ColorConsole console = new ColorConsole();
            token.WriteLine(console);


            foreach (Rule<T> rule in Rules) {


                if (!rule.Check(ref stack)) {
                    result = null;
                    return false;
                }
            }

            result = Construct?.Invoke(null) ?? null;
            return true;
        }

        public bool IsMatch(Token<T> token) {
            return token.TokenType.Equals(TriggerToken) && (TriggerPattern == null || Regex.IsMatch(token.Value, TriggerPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
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
