namespace Maxstupo.Fsu.Core.Format {

    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using System;

    public class FormatToken {

        public string Value { get; }

        public PropertyType Type { get; }

        public int Decimals { get; }

        public int Padding { get; }

        public string DecimalFormat { get; }

        public string Unit { get; }

        public bool IsText => Type == PropertyType.Text;

        public FormatToken(string value, PropertyType type, int decimals = 2, int padding = 0, string unit = null) {
            Value = value ?? throw new ArgumentNullException(nameof(value));

            Type = type;

            Decimals = decimals;
            Padding = padding;

            Unit = unit;
            DecimalFormat = $"{new string('0', padding + 1)}.{new string(decimals > 0 ? '0' : '#', Math.Abs(Decimals))}";
        }

        public PropertyItem GetProperty(IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
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