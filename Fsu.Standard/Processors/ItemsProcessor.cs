using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;
using System.Linq;
namespace Maxstupo.Fsu.Standard.Processor {
    public class ItemsProcessor : IProcessor {
        private readonly IEnumerable<string> newItems;

        public ItemsProcessor(IEnumerable<string> newItems) {
            this.newItems = newItems;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            return items.Concat(newItems.Select(x => new ProcessorItem(x)));
        }

        public override string ToString() {
            return $"{nameof(ItemsProcessor)}[{string.Join(", ", newItems)}]";
        }

    }
}
