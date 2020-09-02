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

        public enum TokenTypeTestAuto {
            A,
            B,
            C,
            [TokenDef(@"S\d\d")]
            D,
            [TokenDef(@"E\d\d")]
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
        public void Add_Count_ExpectsOne() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, ""));

            Assert.Single(tokenizer.TokenDefinitions);
        }

        [Fact]
        public void LoadTokenDefinitions_Count_ExpectsTwo() {
            var tokenizer = new Tokenizer<TokenTypeTestAuto>(TokenTypeTestAuto.A, TokenTypeTestAuto.B, TokenTypeTestAuto.C, false);
            tokenizer.LoadTokenDefinitions();

            Assert.Equal(2, tokenizer.TokenDefinitions.Count);
        }

        [Fact]
        public void LoadTokenDefinitionsCtor_Count_ExpectsTwo() {
            var tokenizer = new Tokenizer<TokenTypeTestAuto>(TokenTypeTestAuto.A, TokenTypeTestAuto.B, TokenTypeTestAuto.C, true);
            Assert.Equal(2, tokenizer.TokenDefinitions.Count);
        }

        [Fact]
        public void Remove_NonExistingTokenDefinition_ExpectsNoChange() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);

            var td1 = new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, "");
            tokenizer.Add(td1);

            var td2 = new TokenDefinition<TokenTypeTest>(TokenTypeTest.E, "");
            tokenizer.Remove(td2);

            Assert.Single(tokenizer.TokenDefinitions);
        }

        [Fact]
        public void Remove_ExistingTokenDefinition_ExpectsDecrease() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);

            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, "");
            tokenizer.Add(td);
            tokenizer.Remove(td);

            Assert.DoesNotContain(td, tokenizer.TokenDefinitions);
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

        [Fact]
        public void Clear_AddTokenDef_ExpectsZero() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.A, TokenTypeTest.B, TokenTypeTest.C, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"S\d\d"));

            tokenizer.Clear();

            Assert.Empty(tokenizer.TokenDefinitions);
        }

    }

#pragma warning restore IDE0008 // Use explicit type

}