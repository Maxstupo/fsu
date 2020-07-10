namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System.Collections.Generic;
    using System.IO;
    using Maxstupo.Fsu.Core.Processor;

    public class MkDirProcessor : IProcessor {

        public MkDirProcessor() {

        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            foreach (ProcessorItem item in items) {
                Directory.CreateDirectory(item.Value);
                yield return item;
            }
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }

}