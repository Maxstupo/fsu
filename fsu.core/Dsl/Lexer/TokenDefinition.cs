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
        public string Regex { get; }

        /// <summary>
        /// A template string for formatting the output value of this token. Access to all regex groups captured are available. Set empty to disable. <br/> See <seealso cref="string.Format(string, object[])"/>
        /// </summary>
        public string Template { get; }

        /// <summary>The precedence when multiple matches are found on the same index. Lower numbers are higher ranking.</summary>
        public int Precedence { get; }

        /// <summary>A hint indicating if this definition has variable values (e.g. number or quoted text).</summary>
        public bool HasVariableValue { get; }

        /// <summary>
        /// Retargets the match of this token to the specified group. Added as a workaround for zero-width positive lookbehind assertions not supporting quantifiers.
        /// Set to zero or less to disable.
        /// </summary>
        public int RetargetToGroup { get; } = -1;

        public string RemoveRegex { get; }

        private readonly Regex regex;
        private readonly Regex removeRegex;

        public TokenDefinition(T tokenType, string regex, string template = null, int precedence = 1, bool hasVariableValue = true, string removeRegex = null, int retargetToGroup = -1) {
            TokenType = tokenType;
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Template = template;
            Precedence = precedence;
            HasVariableValue = hasVariableValue;
            RemoveRegex = removeRegex;
            RetargetToGroup = retargetToGroup;
            //TODO: Move RegexOptions into variable.
            this.regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            if (!string.IsNullOrWhiteSpace(RemoveRegex))
                this.removeRegex = new Regex(RemoveRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Finds all matches of this <see cref="TokenDefinition{T}"/> within the provided input string.
        /// </summary>
        public IEnumerable<Token<T>> FindMatches(string input, int lineNumber) {
            return regex.Matches(input).Cast<Match>().Where(x => x.Success).Select(match => {

                int startIndex = match.Index;
                int endIndex = startIndex + match.Length;

                string value = match.Value;

                if (RetargetToGroup > 0) {
                    startIndex = match.Groups[RetargetToGroup].Index;
                    endIndex = startIndex + match.Groups[RetargetToGroup].Length;
                    value = match.Groups[RetargetToGroup].Value;
                }

                if (!string.IsNullOrWhiteSpace(Template)) {
                    string[] values = match.Groups.Cast<Group>().Where(x => x.Success).Select(x => x.Value).ToArray();
                    value = string.Format(Template, values);
                } else if (match.Groups.Count > 1) {
                    Group group = match.Groups[1];
                    if (group.Success)
                        value = group.Value;
                }

                if (removeRegex != null)
                    value = removeRegex.Replace(value, string.Empty);

                return new Token<T>(TokenType, value, startIndex, endIndex, Precedence, HasVariableValue, lineNumber);

            });
        }

        public override bool Equals(object obj) {
            return Equals(obj as TokenDefinition<T>);
        }

        public bool Equals(TokenDefinition<T> other) {
            return other != null &&
                   EqualityComparer<T>.Default.Equals(TokenType, other.TokenType) &&
                   Regex == other.Regex;
        }

        public override int GetHashCode() {
            var hashCode = -1694758725;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(TokenType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Regex);
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

        /// <inheritdoc cref="TokenDefinition{T}.Regex"/>
        public string Regex { get; }

        /// <inheritdoc cref="TokenDefinition{T}.Precedence"/>
        public int Precedence { get; }

        /// <inheritdoc cref="TokenDefinition{T}.Template"/>
        public string Template { get; set; } = null;

        /// <inheritdoc cref="TokenDefinition{T}.HasVariableValue"/>
        public bool HasVariableValue { get; set; } = true;

        public string RemoveRegex { get; set; } = null;

        /// <inheritdoc cref="TokenDefinition{T}.RetargetToGroup"/>
        public int RetargetToGroup { get; set; } = -1;

        public TokenDef(string regex, int precedence = 1) {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Precedence = precedence;
        }

    }

}
