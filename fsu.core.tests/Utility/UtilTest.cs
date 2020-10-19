namespace Maxstupo.Fsu.Core.Utility.Tests {

    using Xunit;

#pragma warning disable IDE0008 // Use explicit type

    public class UtilTest {


        [Theory]
        [InlineData("Hello World", 11)]
        [InlineData("Hello World", -1)]
        public void SafeSubstring_OutOfRange_ExpectsUnchangedString(string text, int startIndex) {
            string result = text.SafeSubstring(startIndex);

            Assert.Equal(text, result);
        }

        [Fact]
        public void SafeSubstring_EmptyString_ExpectsUnchangedString() {
            string result = string.Empty.SafeSubstring(2);

            Assert.Equal(string.Empty, result);
        }

    }

#pragma warning restore IDE0008 // Use explicit type

}