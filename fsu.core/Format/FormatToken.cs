using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using System;

namespace Maxstupo.Fsu.Core.Format {

    public class FormatToken {

        public string Value { get; }

        public PropertyType Type { get; }

        public int Decimals { get; }

        public Unit Unit { get; }

        public bool IsText => Type == PropertyType.Text;

        public FormatToken(string value, PropertyType type, int decimals = 2, Unit unit = Unit.None) {
            Value = value ?? throw new ArgumentNullException(nameof(value));

            Type = type;

            Decimals = decimals;
            Unit = unit;
        }

        public Property GetProperty(IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            switch (Type) {

                case PropertyType.Global:
                    return propertyStore.GetProperty(Value);

                case PropertyType.Item:
                    return item.GetProperty(propertyProvider, Value);

                case PropertyType.Text:
                default:
                    return null;
            }
        }

    }

}
