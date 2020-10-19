namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;

    public class IndexProcessor : IProcessor {

        public IndexProcessor() {
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            int i = 1;
            foreach (ProcessorItem item in items) {
                item.TryCachePropertyValue("index", new PropertyItem(i++, null));
                yield return item;
            }
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }

}