namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Utility;

    /// <summary>
    /// Represents a sub-string of the string provided by the <see cref="ITokenizer{T}.Tokenize(string, int)"/> method.
    /// </summary>
    /// <typeparam name="T">The enum type that represent this token.</typeparam>
    public class Token<T> : IEquatable<Token<T>> where T : Enum {

        /// <summary>The enum value this token represents.</summary>
        public T TokenType { get; }

        /// <summary>The substring of this token.</summary>
        public string Value { get; }

        /// <summary>The starting index the token value was from.</summary>
        public int StartIndex { get; }

        /// <summary>The end index the token value was from.</summary>
        public int EndIndex { get; }

        /// <summary>The <see cref="TokenDefinition{T}.Precedence"/> when multiple matches are found on the same index. Lower numbers are higher ranking.</summary>
        public int Precedence { get; }

        /// <summary>A hint indicating if this token has a variable value (e.g. number or quoted text).</summary>
        public bool HasVariableValue { get; }

        /// <summary>The line number this token was on. Will be -1 if line number isn't applicable for this token.</summary>
        public int LineNumber { get; }

        /// <summary>The file location of this token, formatted: #linenumber @ start-end </summary>
        public string Location => $"#{LineNumber} @ {StartIndex}-{EndIndex}";

        /// <summary>
        /// End of file ctor.
        /// </summary>
        public Token(T tokenType, int lineNumber) : this(tokenType, string.Empty, -1, -1, 1, false, lineNumber) { }

        /// <summary>
        /// End of line ctor.
        /// </summary>
        /// <param name="index">The start and end indexes.</param>
        public Token(T tokenType, int index, int lineNumber) : this(tokenType, string.Empty, index, index, 1, false, lineNumber) { }

        /// <summary>
        /// Invalid tokens ctor.
        /// </summary>
        public Token(T tokenType, string value, int startIndex, int endIndex, int lineNumber) : this(tokenType, value, startIndex, endIndex, 1, true, lineNumber) { }

        public Token(T tokenType, string value, int startIndex, int endIndex, int precedence, bool hasVariableValue, int lineNumber) {
            TokenType = tokenType;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            StartIndex = startIndex;
            EndIndex = endIndex;
            Precedence = precedence;
            HasVariableValue = hasVariableValue;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Debug method for printing out this token with color coding using <see cref="ColorConsole"/>.<br/>
        /// Expects default(T) to be the invalid token type.
        /// </summary>
        public void WriteLine(IConsole console, char color = 'a') {


            string common = $"&-9;{Precedence}&-^; #&-c;{LineNumber}&-^; @&-c;{StartIndex}&-^;-&-c;{EndIndex}&-^;";
            string value = HasVariableValue ? $"&-e;{Value,-30}&-^;" : $"{"---",-30}";


            if (default(T).Equals(TokenType))
                color = 'c';

            console.WriteLine($"&-{color};{TokenType,-20}&-^;{value}{common}");

        }

        public override bool Equals(object obj) {
            return Equals(obj as Token<T>);
        }

        public bool Equals(Token<T> other) {
            return other != null &&
                   EqualityComparer<T>.Default.Equals(TokenType, other.TokenType) &&
                   Value == other.Value &&
                   StartIndex == other.StartIndex &&
                   EndIndex == other.EndIndex &&
                   LineNumber == other.LineNumber &&
                   Location == other.Location;
        }

        public override int GetHashCode() {
            var hashCode = -735772877;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(TokenType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + StartIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + EndIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + LineNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Location);
            return hashCode;
        }

        public static bool operator ==(Token<T> left, Token<T> right) {
            return EqualityComparer<Token<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(Token<T> left, Token<T> right) {
            return !(left == right);
        }

    }

}