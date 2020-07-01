namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Filtering;
    using Maxstupo.Fsu.Core.Processor;

    public class FilterProcessor : IProcessor {

        private readonly Filter filter;

        public FilterProcessor(Filter filter) {
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            return items.Where(item => filter.Check(pipeline.Console, pipeline.PropertyProvider, pipeline.PropertyStore, item));
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }
}
