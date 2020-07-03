namespace Maxstupo.Fsu.Core.Filtering {

    using System;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public enum OperandType {
        ItemProperty,
        GlobalProperty,
        NumericConstant,
        TextConstant
    }

    public sealed class Operand {
        
        public object Value { get; }

        public OperandType Type { get; }

        public Enum Unit { get; }

        private string ValueText => (string) Value;
        private double ValueNumeric => (double) Value;

        public Operand(string value, OperandType type, Enum unit = null) {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.Type = type;
            this.Unit = unit;
        }

        public Operand(double value) {
            this.Value = value;
            this.Type = OperandType.NumericConstant;
        }

        public PropertyItem GetValue(IOutput output, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {

            switch (Type) {
                case OperandType.ItemProperty:
                    PropertyItem propertyItem = item.GetProperty(propertyProvider, ValueText);

                    if (propertyItem != null)
                        return propertyItem;

                    output.WriteLine(Level.Fine, $"  &-c;Item property named '&-e;{ValueText}&-^;' doesn't exist...&-^;");
                    return null;

                case OperandType.GlobalProperty:
                    PropertyItem property = propertyStore.GetProperty(ValueText);
                    if (property != null)
                        return property;

                    output.WriteLine(Level.Fine, $"  &-c;Global property named '&-e;{ValueText}&-^;' doesn't exist...&-^;");
                    return null;

                case OperandType.NumericConstant:
                    return new PropertyItem(ValueNumeric, Unit);

                case OperandType.TextConstant:
                    return new PropertyItem(ValueText);

                default:
                    return null;

            }

        }

    }

}