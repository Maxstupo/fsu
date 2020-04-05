using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Standard.Processor {
    public class TransformProcessor : IProcessor {

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            return items;
        }

        public override string ToString() {
            return $"{nameof(TransformProcessor)}";
        }

    }
}
