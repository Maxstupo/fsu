using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    public class TokenDefinition<T> where T : Enum {

        public T TokenType { get; }

        public string Pattern { get; }

        public string Template { get; }

        public int Precedence { get; }

        public bool HasVariableValue { get; }


        private readonly Regex regex;

        public TokenDefinition(T tokenType, string pattern, string template, int precedence = 1, bool hasVariableValue = true) {
            TokenType = tokenType;
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Precedence = precedence;
            HasVariableValue = hasVariableValue;

            regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        public IEnumerable<Token<T>> FindMatches(string input, int lineNumber) {
            return regex.Matches(input).Cast<Match>().Select(match => {

                if (!match.Success)
                    return null;

                int startIndex = match.Index;
                int endIndex = startIndex + match.Length;

                string value = match.Value;


                if (!string.IsNullOrEmpty(Template)) {
                    string[] values = match.Groups.Cast<Group>().Where(x => x.Success).Select(x => x.Value).ToArray();
                    value = string.Format(Template, values);
                } else if (match.Groups.Count > 1) {
                    Group group = match.Groups[1];
                    if (group.Success)
                        value = group.Value;
                }


                return new Token<T>(TokenType, value, startIndex, endIndex, Precedence, HasVariableValue, lineNumber);

            }).Where(x => x != null);
        }

    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TokenDef : Attribute {

        public string Regex { get; }


        public int Precedence { get; }

        public string Template { get; set; } = string.Empty;

        public bool HasVariableValue { get; set; } = true;

        public TokenDef(string regex, int precedence = 1) {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Precedence = precedence;
        }

    }

}
