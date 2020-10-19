namespace Maxstupo.Fsu.Core.Processor.Tests {

    using System;
    using Maxstupo.Fsu.Core.Detail;
    using Moq;
    using Xunit;

#pragma warning disable IDE0008 // Use explicit type

    public class ProcessorItemTest {

        [Fact]
        public void Ctor_NullValue_ThrowsArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => new ProcessorItem(null, null));
        }

        [Fact]
        public void InitalValue_CtorValue_Equals() {
            var item = new ProcessorItem("Testing123", null);

            Assert.Equal("Testing123", item.InitialValue);
        }

        [Fact]
        public void GetProperty_NullPropertyProvider_ThrowsArgumentNullException() {
            var item = new ProcessorItem("Testing123", null);

            Assert.Throws<ArgumentNullException>(() => item.GetProperty(null, "filesize"));
        }

        [Fact]
        public void TryCachePropertyValue_NullPropertyItem_ThrowsArgumentNullException() {
            var item = new ProcessorItem("Testing123", null);

            Assert.Throws<ArgumentNullException>(() => item.TryCachePropertyValue("file", null));
        }

        [Fact]
        public void TryCachePropertyValue_NullPropertyName_ThrowsArgumentNullException() {
            var item = new ProcessorItem("Testing123", null);

            Assert.Throws<ArgumentNullException>(() => item.TryCachePropertyValue(null, new PropertyItem(21.23)));
        }

        [Fact]
        public void GetProperty_CachedProperty_Equals() {
            var mock = new Mock<IPropertyProvider>();
            mock.Setup(foo => foo.GetProperty(It.IsAny<IPropertyProvider>(), It.IsAny<ProcessorItem>(), "filesize")).Returns<PropertyItem>(null);

            var item = new ProcessorItem("Testing123", null);
            item.TryCachePropertyValue("filesize", new PropertyItem(21.23));

            var property = item.GetProperty(mock.Object, "filesize");

            Assert.NotNull(property); // If null, TryCachePropertyValue might not have worked.
            Assert.Equal(21.23, property.Value);
        }

        [Fact]
        public void GetProperty_ProvidedProperty_Equals() {
            var mock = new Mock<IPropertyProvider>();
            mock.Setup(foo => foo.GetProperty(It.IsAny<IPropertyProvider>(), It.IsAny<ProcessorItem>(), "filesize")).Returns(new PropertyItem(21.23));

            var item = new ProcessorItem("Testing123", null);

            var property = item.GetProperty(mock.Object, "filesize");

            Assert.NotNull(property); // If null, TryCachePropertyValue might not have worked.
            Assert.Equal(21.23, property.Value);
        }

        [Fact]
        public void GetProperty_Origin_IsCached() {
            var mock = new Mock<IPropertyProvider>();
            mock.Setup(foo => foo.GetProperty(It.IsAny<IPropertyProvider>(), It.IsAny<ProcessorItem>(), "origin")).Returns<PropertyItem>(null);

            var item = new ProcessorItem("Testing123", "apple");

            var property = item.GetProperty(mock.Object, "origin");

            Assert.NotNull(property); // If null, TryCachePropertyValue might not have worked.
            Assert.Equal("apple", property.Value);
        }

        [Fact]
        public void GetProperty_Origin_NotCachedProvided() {
            var mock = new Mock<IPropertyProvider>();
            mock.Setup(foo => foo.GetProperty(It.IsAny<IPropertyProvider>(), It.IsAny<ProcessorItem>(), "origin")).Returns(new PropertyItem("fruit"));

            var item = new ProcessorItem("Testing123", null);

            var property = item.GetProperty(mock.Object, "origin");

            Assert.Equal("fruit", property.Value);
        }

        [Fact]
        public void Value_Changed_CacheCleared() {
            var mock = new Mock<IPropertyProvider>();
            mock.Setup(foo => foo.GetProperty(It.IsAny<IPropertyProvider>(), It.IsAny<ProcessorItem>(), "filesize")).Returns<PropertyItem>(null);

            var item = new ProcessorItem("Testing123", null);
            item.TryCachePropertyValue("filesize", new PropertyItem(21.23));

            item.Value = "Done.";

            var property = item.GetProperty(mock.Object, "filesize");

            Assert.Null(property);
        }


    }


#pragma warning restore IDE0008 // Use explicit type

}