namespace Maxstupo.Fsu.Core.Dsl.Lexer.Tests {

    using System;
    using System.Linq;
    using Xunit;

#pragma warning disable IDE0008 // Use explicit type

    public class TokenizerTest {

        public enum TokenTypeTest {
            Invalid,
            Eol,
            Eof,
            D,
            E
        }

        public enum TokenTypeTestAuto {
            Invalid,
            Eol,
            Eof,
            [TokenDef(@"S\d\d")]
            D,
            [TokenDef(@"E\d\d")]
            E
        }

        [Fact]
        public void Add_NullTokenDefinition_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            Assert.Throws<ArgumentNullException>(() => tokenizer.Add(null));
        }

        [Fact]
        public void Remove_NullTokenDefinition_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            Assert.Throws<ArgumentNullException>(() => tokenizer.Remove(null));
        }

        [Fact]
        public void FindAllTokenMatches_NullInput_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            Assert.Throws<ArgumentNullException>(() => tokenizer.FindAllTokenMatches(null, 1).ToList());
        }

        [Theory]
        [InlineData(TokenTypeTest.Invalid, TokenTypeTest.Invalid, TokenTypeTest.Eol)]
        [InlineData(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eol)]
        [InlineData(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Invalid)]
        public void Ctor_DuplicateReservedTokens_ThrowsArgumentException(TokenTypeTest invalid, TokenTypeTest eol, TokenTypeTest eof) {
            Assert.Throws<ArgumentException>(() => new Tokenizer<TokenTypeTest>(invalid, eol, eof, false));
        }

        [Fact]
        public void Add_DuplicateTokenDefinition_ThrowsArgumentException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, "");
            tokenizer.Add(td);

            Assert.Throws<ArgumentException>(() => tokenizer.Add(td));
        }

        [Theory]
        [InlineData(TokenTypeTest.Invalid)]
        [InlineData(TokenTypeTest.Eol)]
        [InlineData(TokenTypeTest.Eof)]
        public void Add_ReservedTokenDefinition_ThrowsArgumentException(TokenTypeTest token) {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            var td = new TokenDefinition<TokenTypeTest>(token, "");
            Assert.Throws<ArgumentException>(() => tokenizer.Add(td));
        }

        [Fact]
        public void Add_Count_ExpectsOne() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, ""));

            Assert.Single(tokenizer.TokenDefinitions);
        }

        [Fact]
        public void LoadTokenDefinitions_Count_ExpectsTwo() {
            var tokenizer = new Tokenizer<TokenTypeTestAuto>(TokenTypeTestAuto.Invalid, TokenTypeTestAuto.Eol, TokenTypeTestAuto.Eof, false);
            tokenizer.LoadTokenDefinitions();

            Assert.Equal(2, tokenizer.TokenDefinitions.Count);
        }

        [Fact]
        public void LoadTokenDefinitionsCtor_Count_ExpectsTwo() {
            var tokenizer = new Tokenizer<TokenTypeTestAuto>(TokenTypeTestAuto.Invalid, TokenTypeTestAuto.Eol, TokenTypeTestAuto.Eof, true);
            Assert.Equal(2, tokenizer.TokenDefinitions.Count);
        }

        [Fact]
        public void Remove_NonExistingTokenDefinition_ExpectsNoChange() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            var td1 = new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, "");
            tokenizer.Add(td1);

            var td2 = new TokenDefinition<TokenTypeTest>(TokenTypeTest.E, "");
            tokenizer.Remove(td2);

            Assert.Single(tokenizer.TokenDefinitions);
        }

        [Fact]
        public void Remove_ExistingTokenDefinition_ExpectsDecrease() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            var td = new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, "");
            tokenizer.Add(td);
            tokenizer.Remove(td);

            Assert.DoesNotContain(td, tokenizer.TokenDefinitions);
        }

        [Fact]
        public void Tokenize_NullInput_ThrowsArgumentNullException() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
            Assert.Throws<ArgumentNullException>(() => tokenizer.Tokenize(null, 1).ToList());
        }

        [Fact]
        public void TokenizeEnumerable_EmptyInput_ExpectsEof() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);

            var matches = tokenizer.Tokenize(Enumerable.Empty<string>()).ToList();

            Assert.Single(matches);
            Assert.Equal(TokenTypeTest.Eof, matches[0].TokenType);
        }

        [Fact]
        public void Tokenize_MultipleMatches_ExpectsOrderedByPresidence() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.E, @"hellowo", precedence: 2));
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"helloworld", precedence: 1));

            var matches = tokenizer.Tokenize("helloworld", 1).ToList();

            Assert.Equal(TokenTypeTest.D, matches[0].TokenType);
        }

        [Fact]
        public void Tokenize_InvalidInputMiddle_ExpectsInvalidType() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.E, @"e"));
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"d"));

            var matches = tokenizer.Tokenize("eabcd", 1).ToList();

            var token = matches[1];
            Assert.Equal(TokenTypeTest.Invalid, token.TokenType);
            Assert.Equal("abc", token.Value);
        }

        [Fact]
        public void Tokenize_InvalidInputStart_ExpectsInvalidType() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.E, @"e"));
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"d"));

            var matches = tokenizer.Tokenize("abced", 1).ToList();

            var token = matches[0];
            Assert.Equal(TokenTypeTest.Invalid, token.TokenType);
            Assert.Equal("abc", token.Value);
        }

        [Fact]
        public void FindAllTokenMatches_MultipleMatches_ExpectedCountAndValue() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
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
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"S\d\d"));

            var matches = tokenizer.Tokenize("MyVideoS23E13OtherE12S11", 1).ToList();
            var lastToken = matches.Last();

            Assert.Equal(TokenTypeTest.Eol, lastToken.TokenType);
        }

        [Fact]
        public void Clear_AddTokenDef_ExpectsZero() {
            var tokenizer = new Tokenizer<TokenTypeTest>(TokenTypeTest.Invalid, TokenTypeTest.Eol, TokenTypeTest.Eof, false);
            tokenizer.Add(new TokenDefinition<TokenTypeTest>(TokenTypeTest.D, @"S\d\d"));

            tokenizer.Clear();

            Assert.Empty(tokenizer.TokenDefinitions);
        }

    }

#pragma warning restore IDE0008 // Use explicit type

}