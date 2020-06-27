namespace Maxstupo.Fsu.Processors {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Maxstupo.Fsu.Core.Processor;

    public class InProcessor : IProcessor {

        private readonly string path;

        public InProcessor(string path) {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            if (!File.Exists(path))
                return items;

            IEnumerable<ProcessorItem> fileItems = File.ReadLines(path, Encoding.UTF8).Select(x => new ProcessorItem(x));
            return items.Concat(fileItems);
        }

        public override string ToString() {
            return $"{GetType().Name}[path='{path}']";
        }

    }
}
