namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;

    public class SortProcessor : IProcessor {

        private readonly string itemPropertyName;
        private readonly bool isDescending;

        public SortProcessor(string itemPropertyName, bool isDescending) {
            this.itemPropertyName = itemPropertyName;
            this.isDescending = isDescending;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            object selector(ProcessorItem item) {
                PropertyItem property = item.GetProperty(pipeline.PropertyProvider, itemPropertyName);
                return property?.ValueNumber ?? 0;
            }

            return isDescending ? items.OrderByDescending(selector) : items.OrderBy(selector);
        }

        public override string ToString() {
            return $"{GetType().Name}[itemPropertyName={itemPropertyName}]";
        }

    }


}
