namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A definition that defines what is required for a given <see cref="Token{T}"/>.
    /// </summary>
    /// <typeparam name="T">The enum type representing this token.</typeparam>
    public class TokenDefinition<T> : IEquatable<TokenDefinition<T>> where T : Enum {

        /// <summary>The enum value this definition represents.</summary>
        public T TokenType { get; }

        /// <summary>The regex pattern.</summary>
        public string Regex { get; }

        /// <summary>
        /// A template string for formatting the output value of this token. Access to all regex groups captured are available. Null if disabled. <br/> See <seealso cref="string.Format(string, object[])"/>
        /// </summary>
        public string Template { get; }

        /// <summary>The precedence when multiple matches are found on the same index. Lower numbers are higher ranking.</summary>
        public int Precedence { get; }

        /// <summary>A hint indicating if this definition has variable values (e.g. number or quoted text).</summary>
        public bool HasVariableValue { get; }

        /// <summary>
        /// Retargets the match of this token to the specified group. Added as a workaround for zero-width positive lookbehind assertions not supporting quantifiers.
        /// Set to -1 to disable.
        /// </summary>
        public int RetargetToGroup { get; } = -1;

        /// <summary>If true regex matching will ignore case.</summary>
        public bool IgnoreCase { get; }

        /// <summary>All text matching this regex will be removed. Null if disabled.</summary>
        public string RemoveRegex { get; }

        private readonly Regex regex;
        private readonly Regex removeRegex;

        public TokenDefinition(T tokenType, string regex, string template = null, int precedence = 1, bool hasVariableValue = true, string removeRegex = null, int retargetToGroup = -1, bool ignoreCase = true) {
            TokenType = tokenType;
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Template = template;
            Precedence = precedence;
            HasVariableValue = hasVariableValue;
            RemoveRegex = removeRegex;
            RetargetToGroup = retargetToGroup;
            IgnoreCase = ignoreCase;

            RegexOptions options = (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.Compiled | RegexOptions.CultureInvariant;

            this.regex = new Regex(Regex, options);
            if (RemoveRegex != null)
                this.removeRegex = new Regex(RemoveRegex, options);
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

                } else if (!string.IsNullOrWhiteSpace(Template)) {
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

        #region Equals, GetHashCode, ==, !=

        public override bool Equals(object obj) {
            return Equals(obj as TokenDefinition<T>);
        }

        public bool Equals(TokenDefinition<T> other) {
            return other != null &&
                   EqualityComparer<T>.Default.Equals(this.TokenType, other.TokenType) &&
                   this.Regex == other.Regex &&
                   this.Template == other.Template &&
                   this.Precedence == other.Precedence &&
                   this.HasVariableValue == other.HasVariableValue &&
                   this.RetargetToGroup == other.RetargetToGroup &&
                   this.IgnoreCase == other.IgnoreCase &&
                   this.RemoveRegex == other.RemoveRegex;
        }

        public override int GetHashCode() {
            int hashCode = -1676834048;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.TokenType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Regex);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Template);
            hashCode = hashCode * -1521134295 + this.Precedence.GetHashCode();
            hashCode = hashCode * -1521134295 + this.HasVariableValue.GetHashCode();
            hashCode = hashCode * -1521134295 + this.RetargetToGroup.GetHashCode();
            hashCode = hashCode * -1521134295 + this.IgnoreCase.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.RemoveRegex);
            return hashCode;
        }

        public static bool operator ==(TokenDefinition<T> left, TokenDefinition<T> right) {
            return EqualityComparer<TokenDefinition<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(TokenDefinition<T> left, TokenDefinition<T> right) {
            return !(left == right);
        }

        #endregion

    }


    /// <summary>
    ///  A attribute definition that defines what is required for a given <see cref="Token{T}"/>.
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

        /// <inheritdoc cref="TokenDefinition{T}.RemoveRegex"/>
        public string RemoveRegex { get; set; } = null;

        /// <inheritdoc cref="TokenDefinition{T}.RetargetToGroup"/>
        public int RetargetToGroup { get; set; } = -1;

        /// <inheritdoc cref="TokenDefinition{T}.IgnoreCase"/>
        public bool IgnoreCase { get; set; } = true;

        public TokenDef(string regex, int precedence = 1) {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Precedence = precedence;
        }

    }

}