using System;

namespace Maxstupo.Fsu.Core.Detail {

    public class Property {

        public object Value { get; private set; }

        public string ValueText { get => Value as string; set => Value = value; }

        public double ValueNumber => IsNumeric ? Convert.ToDouble(Value) : throw new InvalidCastException("Failed to get number value for property!");

        public bool IsNumeric { get; }

        public Property(string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsNumeric = false;
        }

        public Property(double value) {
            Value = value;
            IsNumeric = true;
        }

    }

}