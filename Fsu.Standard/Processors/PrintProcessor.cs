using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Standard.Processor {
    public class PrintProcessor : IProcessor {

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            int i = 0;
            foreach (ProcessorItem item in items) {
                pipeline.Console.WriteLine($"&-8;{i++,3}:&-^; {item.Value}");

            }

            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }
}
