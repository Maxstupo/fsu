using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor.Standard {
    public class FilterProcessor : IProcessor {

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            return items;
        }

    }
}
