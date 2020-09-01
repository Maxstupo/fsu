namespace Maxstupo.Fsu.Core.Dsl.Lexer.Tests {

    using System;
    using System.Linq;
    using Xunit;

#pragma warning disable IDE0008 // Use explicit type

    public class TokenizerTest {

        public enum TokenTypeTest {
            A,
            B,
            C,
            D,
            E
        }

        [Fact]
        public void Add_NullTokenDefinition_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);

            Assert.Throws<ArgumentNullException>(() => tokenizer.Add(null));
        }

        [Fact]
        public void Remove_NullTokenDefinition_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);

            Assert.Throws<ArgumentNullException>(() => tokenizer.Remove(null));
        }

        [Fact]
        public void FindAllTokenMatches_NullInput_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);

            Assert.Throws<ArgumentNullException>(() => tokenizer.FindAllTokenMatches(null, 1).ToList());
        }

        [Theory]
        [InlineData(TokenTypeTest.A, TokenTypeTest.A, TokenTypeTest.B)]
        [InlineData(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.B)]
        [InlineData(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.A)]
        public void Ctor_DuplicateReservedTokens_ThrowsArgumentException(TokenTypeTest invalid, TokenTypeTest eol, TokenTypeTest eof) {
            Assert.Throws<ArgumentException>(() => new Tokenizer<TokenTypeTest>(invalid, eol, eof, false));
        }

        [Fact]
        public void Add_DuplicateTokenDefinition_ThrowsArgumentException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);

            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, "");
            tokenizer.Add(td);

            Assert.Throws<ArgumentException>(() => tokenizer.Add(td));
        }

        [Theory]
        [InlineData(TokenTypeTest.A)]
        [InlineData(TokenTypeTest.B)]
        [InlineData(TokenTypeTest.C)]
        public void Add_ReservedTokenDefinition_ThrowsArgumentException(TokenTypeTest token) {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);

            var td = new TokenDefinition<TokenTypeTest>(token, "");
            Assert.Throws<ArgumentException>(() => tokenizer.Add(td));
        }


        [Fact]
        public void Tokenize_NullInput_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);
            Assert.Throws<ArgumentNullException>(() => tokenizer.Tokenize(null, 1).ToList());
        }

        [Fact]
        public void FindAllTokenMatches_MultipleMatches_ExpectedCountAndValue() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"S\d\d"));
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.E, @"E\d\d"));

            var matches = tokenizer.FindAllTokenMatches("MyVideoS23E13OtherE12S11", 1).ToList();

            Assert.Equal(4, matches.Count);

            Assert.Equal("S23", matches[0].Value);
            Assert.Equal("S11", matches[1].Value);
            Assert.Equal("E13", matches[2].Value);
            Assert.Equal("E12", matches[3].Value);
        }

        [Fact]
        public void Tokenize_AnyInputString_EndsWithEolToken() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"S\d\d"));

            var matches = tokenizer.Tokenize("MyVideoS23E13OtherE12S11", 1).ToList();
            var lastToken = matches.Last();

            Assert.Equal(TokenTypeTest.B, lastToken.TokenType);
        }

    }

#pragma warning restore IDE0008 // Use explicit type

}