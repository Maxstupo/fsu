using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("sort")]
    public class SortProcessor : IProcessor {

        private readonly string propertyName;
        private readonly bool descending;

        public SortProcessor(string propertyName, bool descending = false) {
            this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            this.descending = descending;
        }

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            ProcessorItem item = items.FirstOrDefault();
            if (item == null)
                return true;


            items.Sort((i, j) => {
                Property propI = i.GetFileProperty(propertyProvider, propertyName);
                if (propI == null)
                    return -1;

                Property propJ = j.GetFileProperty(propertyProvider, propertyName);
                if (propJ == null)
                    return -1;

                if (propI.IsNumeric && propJ.IsNumeric) {
                    return !descending ? propI.ValueNumber.CompareTo(propJ.ValueNumber) : propJ.ValueNumber.CompareTo(propI.ValueNumber);

                } else if (!propI.IsNumeric && !propJ.IsNumeric) {
                    return !descending ? propI.ValueText.CompareTo(propJ.ValueText) : propJ.ValueText.CompareTo(propI.ValueText);

                } else {
                    throw new NotSupportedException("Cant sort mixed file property types.");
                }
            });


            return true;

        }

        public static object Construct(string name, TokenList list) {
            if (list.Count == 0 || list.Count > 2)
                return $"has unexpected parameters {list.Next().Location}";

            Token token = list.Next();

            if (token.Type != TokenType.Text)
                return $"has invalid parameters {token.Location}";

            bool descending = false;

            if (list.HasNext) {
                Token token2 = list.Next();
                if (token2.Type != TokenType.Text)
                    return $"has invalid parameters {token2.Location}";

                if (!token2.Value.Equals("desc", StringComparison.InvariantCultureIgnoreCase) && !token2.Value.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                    return $"has invalid parameters {token2.Location}";

                descending = token2.Value.Equals("desc", StringComparison.InvariantCultureIgnoreCase);
            }

            string propertyName = token.Value;
            if (propertyName.StartsWith("@"))
                propertyName = propertyName.Substring(1);
            return new SortProcessor(propertyName, descending);
        }

    }

}
