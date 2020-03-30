using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor.Impl.Aggregate {

    [Function("avg")]
    public class AvgProcessor : PropertyProcessor {

        public AvgProcessor(string propertyName) : base(propertyName) {
        }

        protected override double CalculateProperty(IFilePropertyProvider propertyProvider, ref List<ProcessorItem> items) {
            double total = 0;

            foreach(ProcessorItem item in items) {
                Property property = GetProperty(propertyProvider,item);
                if (property == null || !property.IsNumeric)
                    continue;

                total += property.ValueNumber;
            }

            return total / items.Count;
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

            return new AvgProcessor(propertyName);
        }

    }

}
