using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor.Impl.Aggregate {

    [Function("max")]
    public class MaxProcessor : PropertyProcessor {

        public MaxProcessor(string propertyName) : base(propertyName) {
        }

        protected override double CalculateProperty(IFilePropertyProvider propertyProvider, ref List<ProcessorItem> items) {
            double maximum = double.MinValue;

            foreach (ProcessorItem item in items) {
                Property property = GetProperty(propertyProvider, item);
                if (property == null || !property.IsNumeric)
                    continue;

                maximum = Math.Max(maximum, property.ValueNumber);
            }

            return maximum;
        }

        public static object Construct(string _, TokenList list) {
            if (list.Count != 1)
                return $"has unexpected parameters {(!list.HasNext ? string.Empty : list.Next().Location.ToString())}";

            Token token = list.Next();

            if (token.Type != TokenType.Text)
                return $"has invalid parameters {token.Location}";

            string propertyName = token.Value;

            if (propertyName.StartsWith("@"))
                propertyName = propertyName.Substring(1);

            return new MaxProcessor(propertyName);
        }

    }

}
