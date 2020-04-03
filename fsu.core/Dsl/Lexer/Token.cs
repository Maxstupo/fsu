using Maxstupo.Fsu.Core.Utility;
using System;

namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    /// <summary>
    /// Represents a sub-string of the string provided to the <see cref="ITokenizer{T}.Parse(string, int)"/> method.
    /// </summary>
    /// <typeparam name="T">The enum type that represent this token.</typeparam>
    public class Token<T> where T : Enum {

        public T TokenType { get; }

        /// <summary>The substring of this token.</summary>
        public string Value { get; }

        /// <summary></summary>
        public int StartIndex { get; }

        /// <summary></summary>
        public int EndIndex { get; }

        public int Precedence { get; }

        public bool HasVariableValue { get; }

        /// <summary>The line number this token was on. Will be -1 if line number isn't applicable for this token.</summary>
        public int LineNumber { get; }

        // end of file ctor.
        public Token(T tokenType, int lineNumber) : this(tokenType, string.Empty, -1, -1, 1, false, lineNumber) { }

        // end of line ctor.
        public Token(T tokenType, int index, int lineNumber) : this(tokenType, string.Empty, index, index, 1, false, lineNumber) { }

        // Invalid tokens ctor.
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
        public void Print(char color ='a') {


            string common = $"{Precedence} #&-c;{LineNumber}&-^; @&-c;{StartIndex}&-^;-&-c;{EndIndex}&-^;";
            string value = HasVariableValue ? $"&-e;{Value,-30}&-^;" : $"{"---",-30}";


            if (default(T).Equals(TokenType))
                color = 'c';

            ColorConsole.WriteLine($"&-{color};{TokenType,-20}&-^;{value}{common}");

        }

    }

}
