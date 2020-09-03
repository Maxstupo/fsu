namespace Maxstupo.Fsu.Core.Dsl.Lexer.Tests {

    using Xunit;

    public class TokenTest {

        public enum TokenTypeTest : int {
            A = 2,
            B = 4
        }

        [Fact]
        public void GetHashCode_Unchanging_ExpectsEquals() {
            var token = new Token<TokenTypeTest>(TokenTypeTest.B, "Test", 123, 533, 32, true, 12);

            var hashCode = token.GetHashCode();

            Assert.Equal(-1089497087, hashCode);
        }


        // Should use theory with inline data.
        [Fact]
        public void Equals_IdenticalObject_ExpectsTrue() {
            var token1 = new Token<TokenTypeTest>(TokenTypeTest.B, "Test", 123, 533, 32, true, 12);
            var token2 = new Token<TokenTypeTest>(TokenTypeTest.B, "Test", 123, 533, 32, true, 12);

            var isEqual = token1.Equals(token2);

            Assert.True(isEqual);
        }

    }

}