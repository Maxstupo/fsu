using Maxstupo.Fsu.Core.Detail;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor {

    public class ProcessorItem {

        public string Value { get; }

        public ProcessorItem(string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

}
