using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Lex {

    public class Token : IEquatable<Token> {

        public TokenType Type { get; }

        public string Value { get; }

        public int LengthOffset { get; }

        public TokenLocation Location { get; }

        public int Length => Value.Length + LengthOffset;

        public Token(TokenType type, string value, int lengthOffset, TokenLocation location) {
            Type = type;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            LengthOffset = lengthOffset;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public override bool Equals(object obj) {
            return Equals(obj as Token);
        }

        public bool Equals(Token other) {
            return other != null &&
                   Type == other.Type &&
                   Value == other.Value &&
                   LengthOffset == other.LengthOffset &&
                   EqualityComparer<TokenLocation>.Default.Equals(Location, other.Location);
        }

        public override int GetHashCode() {
            int hashCode = 529585983;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + LengthOffset.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TokenLocation>.Default.GetHashCode(Location);
            return hashCode;
        }

        public static bool operator ==(Token left, Token right) {
            return EqualityComparer<Token>.Default.Equals(left, right);
        }

        public static bool operator !=(Token left, Token right) {
            return !(left == right);
        }
    }

}
