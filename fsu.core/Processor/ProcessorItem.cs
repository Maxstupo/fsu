using Maxstupo.Fsu.Core.Detail;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor {

    public class ProcessorItem {

        public string InitalValue { get; }

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

        private readonly Dictionary<string, Property> cachedProperties = new Dictionary<string, Property>();

        public ProcessorItem(string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            InitalValue = value;
        }

        public Property GetFileProperty(IFilePropertyProvider propertyProvider, string propertyName) {
           
            if (propertyName.Equals("megapixels", StringComparison.InvariantCultureIgnoreCase)) {
                Property p1 = GetFileProperty(propertyProvider, "width");
                Property p2 = GetFileProperty(propertyProvider, "height");
                return new Property(p1.ValueNumber * p2.ValueNumber / 1000000);
            }

            if (cachedProperties.TryGetValue(propertyName, out Property property)) {
                return property;
            } else {
                property = propertyProvider.GetFileProperty(this, propertyName);

                if (property != null)
                    cachedProperties.Add(propertyName, property);

                return property;
            }

        }

    }

}
