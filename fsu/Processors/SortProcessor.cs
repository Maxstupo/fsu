using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Processors {
    public class SortProcessor : IProcessor {

        private readonly string itemPropertyName;

        public SortProcessor(string itemPropertyName) {
            this.itemPropertyName = itemPropertyName;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            return items.OrderByDescending(item => {
                PropertyItem property = item.GetProperty(pipeline.PropertyProvider, itemPropertyName);
                return property?.ValueNumber ?? 0;
            });

        }
        public override string ToString() {
            return $"{GetType().Name}[itemPropertyName={itemPropertyName}]";
        }

    }


}
