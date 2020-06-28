namespace Maxstupo.Fsu.Core.Detail.Converters {

    using System;
    using UnitsNet;

    public class UnitNetPropertyConverter : IUnitConverter {
      
        public double ConvertPropertyValue(PropertyItem property, string newUnitAbbr) {
            if (property == null || !property.IsNumeric || property.Unit == null)
                return property.ValueNumber;

            if (!UnitParser.Default.TryParse(newUnitAbbr, property.Unit.GetType(), out Enum newUnit))
                return property.ValueNumber;

            if (Quantity.TryFrom(property.ValueNumber, property.Unit, out IQuantity quantity))
                return quantity.As(newUnit);

            return property.ValueNumber;
        }

    }

}