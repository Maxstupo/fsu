using Maxstupo.Fsu.Core.Detail;
using System;

namespace Maxstupo.Fsu.Core.Processor {

    public class ProcessorItem {

        public string Value { get; set; }

        public ProcessorItem(string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }


        public Property GetProperty(IPropertyProvider propertyProvider, string propertyName) {

            // TODO: Cache item properties.
            return propertyProvider.GetProperty(this, propertyName);

        }
    }

}
