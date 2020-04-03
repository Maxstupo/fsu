using System;

namespace Maxstupo.Fsu.Core.Detail {

    public class Property {

        public object Value { get; private set; }

        public string ValueText { get => Value as string; set => Value = value; }

        public double ValueNumber {
            get => IsNumeric ? Convert.ToDouble(Value) : throw new InvalidCastException("Failed to get number value for non-numeric property!");
            set {
                if (!IsNumeric)
                    throw new InvalidCastException("Failed to set number value for non-numeric property!");
                Value = value;
            }
        }

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