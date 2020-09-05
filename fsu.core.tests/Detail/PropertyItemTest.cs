namespace Maxstupo.Fsu.Core.Detail.Tests {

    using System;
    using Xunit;

#pragma warning disable IDE0008 // Use explicit type

    public class PropertyItemTest {

        [Fact]
        public void Ctor_NullValue_ThrowsArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => new PropertyItem(null));
        }

        [Fact]
        public void Ctor_NumberValue_IsNumeric() {
            var item = new PropertyItem(12.2);

            Assert.True(item.IsNumeric);
        }

        [Fact]
        public void Ctor_StringValue_NotNumeric() {
            var item = new PropertyItem("Apple");

            Assert.False(item.IsNumeric);
        }


        [Fact]
        public void ValueNumber_Zero_NotNumeric() {
            var item = new PropertyItem("Apple");

            Assert.Equal(0, item.ValueNumber);
        }

    }

#pragma warning restore IDE0008 // Use explicit type

}