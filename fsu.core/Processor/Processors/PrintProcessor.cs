namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public class PrintProcessor : IProcessor {

        public PrintProcessor() { }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            int i = 0;
            foreach (ProcessorItem item in items)
                pipeline.Output.WriteLine(Level.None, $"&-8;{i++,3}:&-^; {item.Value}");

            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }
}
