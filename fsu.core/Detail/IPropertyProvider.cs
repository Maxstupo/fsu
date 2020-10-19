namespace Maxstupo.Fsu.Core.Detail {

    using Maxstupo.Fsu.Core.Processor;

    /// <summary>
    /// Provides metadata for processor items.
    /// </summary>
    public interface IPropertyProvider {

        /// <summary>
        /// Acts as a hint for the property provider that a bulk property request (of the same propertyName) is beginning.
        /// </summary>
        void Begin();

        /// <summary>
        /// Acts as a hint for the property provider that a bulk property request is ending.
        /// </summary>
        void End();

        /// <summary>
        /// Returns a property item based off the processor item and property name specified.
        /// </summary>
        /// <param name="propertyProvider">A property provider used to access additional properties (enables the composite properties to be provided e.g. megapixels).</param>
        /// <param name="item">The processor item this property is sourced from.</param>
        /// <param name="propertyName">The property provider dependant name of the property item.</param>
        PropertyItem GetProperty(IPropertyProvider propertyProvider, ProcessorItem item, string propertyName);

    }

}