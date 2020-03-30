using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Lex {

    public class TokenLocation : IEquatable<TokenLocation> {
        public int LineNumber { get; }
        public int ColumnIndex { get; }

        public TokenLocation(int lineNumber, int columnIndex) {
            LineNumber = lineNumber;
            ColumnIndex = columnIndex;
        }


        public override string ToString() {
            return $"@line:{LineNumber + 1},{ColumnIndex}";
        }

        public override bool Equals(object obj) {
            return Equals(obj as TokenLocation);
        }

        public bool Equals(TokenLocation other) {
            return other != null &&
                   LineNumber == other.LineNumber &&
                   ColumnIndex == other.ColumnIndex;
        }

        public override int GetHashCode() {
            var hashCode = -574462973;
            hashCode = hashCode * -1521134295 + LineNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + ColumnIndex.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(TokenLocation left, TokenLocation right) {
            return EqualityComparer<TokenLocation>.Default.Equals(left, right);
        }

        public static bool operator !=(TokenLocation left, TokenLocation right) {
            return !(left == right);
        }
    }

}
