namespace Maxstupo.Fsu.Core.Detail {

    using System;

    public class PropertyItem {

        public object Value { get;  }

        public string ValueText => Value as string;

        public double ValueNumber =>
#if DEBUG
            IsNumeric ? Convert.ToDouble(Value) : throw new InvalidCastException("Failed to get number value for non-numeric property!");
#else
            Convert.ToDouble(Value);
#endif

        public bool IsNumeric { get; }

        public Enum Unit { get; }

        public PropertyItem(string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsNumeric = false;
        }

        public PropertyItem(double value, Enum unit) {
            Value = value;
            Unit = unit;
            IsNumeric = true;
        }

    }

}