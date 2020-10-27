namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;

    public class RenameProcessor : IProcessor {

        private readonly FormatTemplate dstTemplate;

        public RenameProcessor(FormatTemplate dstTemplate) {
            this.dstTemplate = dstTemplate;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            if (pipeline.Simulate)
                pipeline.Output.WriteLine(Utility.Level.Info, $"\n&-c;-------- Simulation Mode Active! --------&-^;\n");

            foreach (ProcessorItem item in items) {
                string srcFilepath = item.Value;
                string srcDir = Path.GetDirectoryName(srcFilepath);

                string dstFilename = dstTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);
                string dstFilepath = Path.Combine(srcDir, dstFilename);

                pipeline.Output.WriteLine(Utility.Level.Info, $"Renaming: &-6;{srcFilepath}&-^; -> &-6;{dstFilepath}&-^;");

                if (!pipeline.Simulate) {
                    if (File.Exists(srcFilepath)) {
                        if (!File.Exists(dstFilepath)) {
                            File.Move(srcFilepath, dstFilepath);
                        } else {
                            pipeline.Output.WriteLine(Utility.Level.Info, $"  - &-c;Destination filepath already exists! Ignoring...&-^;");

                        }
                    } else {
                        pipeline.Output.WriteLine(Utility.Level.Info, $"  - &-c;Source filepath doesn't exist! Ignoring...&-^;");

                    }
                }
            }

            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}[dstTemplate={dstTemplate.Template}]";
        }

    }
}
