using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    /// <summary>
    /// A definition that defines what regex pattern is needed for a given token.
    /// </summary>
    /// <typeparam name="T">The enum type representing each token.</typeparam>
    public class TokenDefinition<T> : IEquatable<TokenDefinition<T>> where T : Enum {

        /// <summary>The enum value this definition represents.</summary>
        public T TokenType { get; }

        /// <summary>The regex pattern.</summary>
        public string Pattern { get; }

        /// <summary>
        /// A template string for formatting the output value of this token. Access to all regex groups captured are available. Set empty to disable. <br/> See <seealso cref="string.Format(string, object[])"/>
        /// </summary>
        public string Template { get; }

        /// <summary>The precedence when multiple matches are found on the same index. Lower numbers are higher ranking.</summary>
        public int Precedence { get; }

        /// <summary>A hint indicating if this definition has variable values (e.g. number or quoted text).</summary>
        public bool HasVariableValue { get; }


        private readonly Regex regex;

        public TokenDefinition(T tokenType, string pattern, string template = null, int precedence = 1, bool hasVariableValue = true) {
            TokenType = tokenType;
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Template = template ?? string.Empty;
            Precedence = precedence;
            HasVariableValue = hasVariableValue;

            //TODO: Move RegexOptions into variable.
            regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Finds all matches of this <see cref="TokenDefinition{T}"/> within the provided input string.
        /// </summary>
        public IEnumerable<Token<T>> FindMatches(string input, int lineNumber) {
            return regex.Matches(input).Cast<Match>().Where(x => x.Success).Select(match => {

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

            });
        }

        public override bool Equals(object obj) {
            return Equals(obj as TokenDefinition<T>);
        }

        public bool Equals(TokenDefinition<T> other) {
            return other != null &&
                   EqualityComparer<T>.Default.Equals(TokenType, other.TokenType) &&
                   Pattern == other.Pattern;
        }

        public override int GetHashCode() {
            var hashCode = -1694758725;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(TokenType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Pattern);
            return hashCode;
        }

        public static bool operator ==(TokenDefinition<T> left, TokenDefinition<T> right) {
            return EqualityComparer<TokenDefinition<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(TokenDefinition<T> left, TokenDefinition<T> right) {
            return !(left == right);
        }
    }


    /// <summary>
    ///  A attribute definition that defines what regex pattern is needed for a given enum token.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TokenDef : Attribute {

        /// <inheritdoc cref="TokenDefinition{T}.Pattern"/>
        public string Regex { get; }

        /// <inheritdoc cref="TokenDefinition{T}.Precedence"/>
        public int Precedence { get; }

        /// <inheritdoc cref="TokenDefinition{T}.Template"/>
        public string Template { get; set; } = string.Empty;

        /// <inheritdoc cref="TokenDefinition{T}.HasVariableValue"/>
        public bool HasVariableValue { get; set; } = true;


        public TokenDef(string regex, int precedence = 1) {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Precedence = precedence;
        }

    }

}
