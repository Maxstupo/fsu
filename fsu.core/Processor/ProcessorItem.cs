namespace Maxstupo.Fsu.Core.Processor {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;

    /// <summary>
    /// Represents an item passed between processors. Each item represents something as a string, this could be a filepath, url, or anything.
    /// ProcessItems remember their inital value, and provide the means to cache properties related to this processor item.
    /// </summary>
    public sealed class ProcessorItem {

        /// <summary>The initial readonly value of this processor item.</summary>
        public string InitialValue { get; }

        private string value;
        /// <summary>The current value of this processor item. Modifiying this value will result in the clearing of all previously cached properties.</summary>
        public string Value {
            get => value;
            set {
                if (this.value != value)
                    this.cachedProperties.Clear();
                this.value = value;
            }
        }

        /// <summary>A cache of property items previously obtained.</summary>
        private readonly Dictionary<string, PropertyItem> cachedProperties = new Dictionary<string, PropertyItem>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Constructs a new ProcessorItem.
        /// </summary>
        /// <param name="origin">An optional value that will set a property value called "origin". Used as a "source" for where this processor item was obtained form.</param>
        public ProcessorItem(string value, string origin = null) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            InitialValue = value;

            if (origin != null)
                TryCachePropertyValue("origin", new PropertyItem(origin));
        }

        /// <summary>
        /// Returns a property item obtained from the property provider given.        
        /// Properties will be cached within this processor item, attempts to get the same property item will result in the cached version being returned.
        /// </summary>
        /// <param name="propertyProvider">The provider to use as the source of property item lookups.</param>
        /// <param name="propertyName">The name of the property to obtain.</param>
        public PropertyItem GetProperty(IPropertyProvider propertyProvider, string propertyName) {

            if (cachedProperties.TryGetValue(propertyName, out PropertyItem property)) {
                return property;
            } else {
                property = propertyProvider.GetProperty(propertyProvider, this, propertyName.ToLowerInvariant());

                TryCachePropertyValue(propertyName, property);

                return property;
            }
        }

        public void TryCachePropertyValue(string propertyName, PropertyItem property) {
            if (propertyName != null && property != null && !cachedProperties.ContainsKey(propertyName))
                cachedProperties.Add(propertyName, property);
        }

    }

}