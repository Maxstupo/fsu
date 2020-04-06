using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Standard.Processor {
    public class PrintProcessor : IProcessor {

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            List<ProcessorItem> pi = items.ToList(); // Ensure previous processors are run.

            int i = 0;
            foreach (ProcessorItem item in pi) {
                pipeline.Console.WriteLine($"&-8;{i++,3}:&-^; {item.Value}");

            }

            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }
}
