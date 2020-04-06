using Maxstupo.Fsu.Core.Detail;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor {

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

        private readonly Dictionary<string, Property> cachedProperties = new Dictionary<string, Property>();

        public ProcessorItem(string value, string origin = null) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            InitialValue = value;
            Origin = origin;
        }

        public Property GetProperty(IPropertyProvider propertyProvider, string propertyName) {
            if (cachedProperties.TryGetValue(propertyName, out Property property)) {
                return property;
            } else {
                property = propertyProvider.GetProperty(this, propertyName);

                if (property != null)
                    cachedProperties.Add(propertyName, property);

                return property;
            }
        }
    }

}
