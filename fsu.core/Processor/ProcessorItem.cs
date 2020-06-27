namespace Maxstupo.Fsu.Core.Processor {

    using Maxstupo.Fsu.Core.Detail;
    using System;
    using System.Collections.Generic;

    public class ProcessorItem {

        public string InitialValue { get; }

        private string _value;
        public string Value {
            get => _value;
            set {
                if (_value != value)
                    cachedProperties.Clear();
                _value = value;
            }
        }

        public string Origin { get; set; }

        private readonly Dictionary<string, PropertyItem> cachedProperties = new Dictionary<string, PropertyItem>();

        public ProcessorItem(string value, string origin = null) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            InitialValue = value;
            Origin = origin;
        }

        public PropertyItem GetProperty(IPropertyProvider propertyProvider, string propertyName) {
            if (cachedProperties.TryGetValue(propertyName, out PropertyItem property)) {
                return property;
            } else {
                property = propertyProvider.GetProperty(propertyProvider, this, propertyName);

                if (property != null)
                    cachedProperties.Add(propertyName, property);

                return property;
            }
        }

        public void TryCachePropertyValue(string propertyName, PropertyItem property) {
            if (property != null)
                cachedProperties.Add(propertyName, property);
        }
 
    }

}