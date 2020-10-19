namespace Maxstupo.Fsu.Core.Dsl.Lexer.Tests {

    using System;
    using System.Linq;
    using Xunit;

#pragma warning disable IDE0008 // Use explicit type

    public class TokenDefinitionTest {

        private enum TokenTypeTest {
            MyToken1,
            MyToken2,
            MyToken3
        }

        [Fact]
        public void Ctor_NullRegex_ThrowsArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken3, null, null, 1, true, null, -1, true));
        }

        [Fact]
        public void TokenCtor_NullRegex_ThrowsArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => new Token<TokenTypeTest>(TokenTypeTest.MyToken3, null, 0, 0, 0, true, 0));
        }

        [Fact]
        public void FindMatches_NullInput_ThrowsArgumentNullException() {
            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken2, ".+", null, 1, true, null, -1, true);

            Assert.Throws<ArgumentNullException>(() => td.FindMatches(null, 1));
        }

        [Theory]
        [InlineData("Hello World mb", 1)]
        [InlineData("mb test gb", 2)]
        [InlineData("testing", 0)]
        [InlineData("mbmbgbgb", 4)]
        public void FindMatches_MatchCount_Equals(string data, int matchCount) {
            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken2, "mb|gb", null, 1, true, null, -1, true);

            var matches = td.FindMatches(data, 1);

            Assert.Equal(matchCount, matches.Count());
        }

        [Theory]
        [InlineData(2, false, "#5 @ 0/11")]
        [InlineData(8, true, "#5 @ 0/11")]
        public void FindMatches_TokenProperties_MatchDefinition(int precedence, bool hasVariableValue, string location) {
            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken2, ".+", null, precedence, hasVariableValue, null, -1, true);

            var matches = td.FindMatches("hello_world", 5);

            foreach (Token<TokenTypeTest> match in matches) {
                Assert.Equal(TokenTypeTest.MyToken2, match.TokenType);
                Assert.Equal(precedence, match.Precedence);
                Assert.Equal(hasVariableValue, match.HasVariableValue);
                Assert.Equal(5, match.LineNumber);
                Assert.Equal(location, match.Location);
            }
        }

        [Fact]
        public void FindMatches_RemoveRegex_Equals() {
            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken2, "hello \\d\\.\\dworld", null, 1, true, "\\d\\.\\d", -1, true);

            var matches = td.FindMatches("testing hello 5.2world", 1);

            Assert.Equal("hello world", matches.First().Value);
        }

        [Fact]
        public void FindMatches_GroupCapture_Equals() {
            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken2, "hello (\\d\\.\\d)world", null, 1, true, null, -1, true);

            var matches = td.FindMatches("testing hello 5.2world", 1);

            Assert.Equal("5.2", matches.First().Value);
        }

        [Fact]
        public void FindMatches_RetargetGroup_Equals() {
            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken2, "S(\\d\\d)E(\\d\\d)", null, 1, true, null, 1, true);

            var matches = td.FindMatches("VideoS01E05.mp4", 1);

            var token = matches.First();
            Assert.Equal("05", token.Value);
            Assert.Equal(9, token.StartIndex);
            Assert.Equal(11, token.EndIndex);
        }

        [Fact]
        public void FindMatches_Template_Equals() {
            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.MyToken2, "S(\\d\\d)E(\\d\\d)", "Movie {1}-{2}", 1, true, null, -1, true);

            var matches = td.FindMatches("testinghelloS21E32world", 1);
            var token = matches.First();

            Assert.Equal("Movie 21-32", token.Value);
            Assert.Equal(12, token.StartIndex);
            Assert.Equal(18, token.EndIndex);
        }

    }

#pragma warning restore IDE0008 // Use explicit type
}
