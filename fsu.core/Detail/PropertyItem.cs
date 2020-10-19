namespace Maxstupo.Fsu.Core.Detail {

    using System;

    /// <summary>
    /// A metadata value of a processor item. Supports both text and numeric values (optionally a numeric unit for conversions).
    /// </summary>
    public sealed class PropertyItem {

        /// <summary>The raw property item value, could be a string or double.</summary>
        public object Value { get; }

        /// <summary>The value in string form.</summary>
        public string ValueText => (string) Value;

        /// <summary>The value in number form. Returns zero if property item isn't numeric.</summary>
        public double ValueNumber => IsNumeric ? Convert.ToDouble(Value) : 0;

        /// <summary>True if this property item represents a number.</summary>
        public bool IsNumeric { get; }

        /// <summary>An optional enum containing the numeric unit the value is in, used for conversions (e.g. GB to KB).</summary>
        public Enum Unit { get; }

        /// <summary>
        /// Constructs a new string PropertyItem.
        /// </summary>
        public PropertyItem(string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsNumeric = false;
        }

        /// <summary>
        /// Constructs a new number PropertyItem.
        /// </summary>
        public PropertyItem(double value, Enum unit = null) {
            Value = value;
            Unit = unit;
            IsNumeric = true;
        }

        public static implicit operator string(PropertyItem propertyItem) => propertyItem.ValueText;
        public static implicit operator double(PropertyItem propertyItem) => propertyItem.ValueNumber;

    }

}