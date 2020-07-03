namespace Maxstupo.Fsu.Core.Processor {

    using Maxstupo.Fsu.Core.Detail;
    using System;
    using System.Collections.Generic;

    public class ProcessorItem {

        public string InitialValue { get; }

        private string value;
        public string Value {
            get => value;
            set {
                if (this.value != value)
                    this.cachedProperties.Clear();
                this.value = value;
            }
        }

        private readonly Dictionary<string, PropertyItem> cachedProperties = new Dictionary<string, PropertyItem>(StringComparer.InvariantCultureIgnoreCase);

        public ProcessorItem(string value, string origin = null) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            InitialValue = value;
            if (origin != null)
                TryCachePropertyValue("origin", new PropertyItem(origin));
        }

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
            if (property != null)
                cachedProperties.Add(propertyName, property);
        }

    }

}