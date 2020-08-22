namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System.Collections.Generic;
    using System.IO;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public class MkDirProcessor : IProcessor {

        private readonly FormatTemplate template;

        public MkDirProcessor(FormatTemplate template) {
            this.template = template;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            if (pipeline.Simulate)
                pipeline.Output.WriteLine(Level.Info, $"\n&-c;-------- Simulation Mode Active! --------&-^;\n");

            foreach (ProcessorItem item in items) {

                string path = template.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);

                pipeline.Output.WriteLine(Level.Info, $"MkDir: &-6;{path}&-^;");

                if (!pipeline.Simulate)
                    Directory.CreateDirectory(path);
            }

            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }

}