using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using System;

namespace Maxstupo.Fsu.Core.Format {

    public enum PropertyType {
        Global,
        File,
        Text
    }

    public class FormatToken {

        public string Value { get; }

        public PropertyType Type { get; }

        public int Decimals { get; }

        public FormatUnit Unit { get; }

        public bool IsText => Type == PropertyType.Text;

        public FormatToken(string value, PropertyType type, int decimals = 2, FormatUnit unit = FormatUnit.None) {
            Value = value ?? throw new ArgumentNullException(nameof(value));

            Type = type;

            Decimals = decimals;
            Unit = unit;
        }

        public Property GetProperty(IFilePropertyProvider propertyProvider, IPropertyStore propertyContainer, ProcessorItem item) {
            if (Type == PropertyType.File) {
                return item.GetFileProperty(propertyProvider, Value);

            } else if (Type == PropertyType.Global) {
                return propertyContainer.GetProperty(Value);

            }

            return null;
        }
    }

}
