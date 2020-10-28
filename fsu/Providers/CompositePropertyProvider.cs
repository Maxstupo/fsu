namespace Maxstupo.Fsu.Providers {

    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;

    /// <summary>
    /// Provides metadata for ProcessorItems derived from other item properties (e.g. megapixels = width*height).
    /// </summary>
    public sealed class CompositePropertyProvider : IPropertyProvider {

        public void Begin() { }

        public void End() { }

        public PropertyItem GetProperty(IPropertyProvider propertyProvider, ProcessorItem item, string propertyName) {

            switch (propertyName) {
                case "pixels":
                    return GetPixels(ref propertyProvider, ref item, 1);

                case "megapixels":
                    return GetPixels(ref propertyProvider, ref item, 1000000);

                default:
                    return null;

            }

        }

        private static PropertyItem GetPixels(ref IPropertyProvider propertyProvider, ref ProcessorItem item, double divisor) {
            PropertyItem propWidth = item.GetProperty(propertyProvider, "width");
            if (propWidth == null || !propWidth.IsNumeric)
                return null;

            PropertyItem propHeight = item.GetProperty(propertyProvider, "height");
            if (propHeight == null || !propHeight.IsNumeric)
                return null;

            return new PropertyItem(propWidth.ValueNumber * propHeight.ValueNumber / divisor);

        }

    }

}