namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;

    public class CopyProcessor : IProcessor {

        private readonly FormatTemplate dstTemplate;

        public CopyProcessor(FormatTemplate dstTemplate) {
            this.dstTemplate = dstTemplate;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            if (pipeline.Simulate)
                pipeline.Output.WriteLine(Utility.Level.Info, $"\n&-c;-------- Simulation Mode Active! --------&-^;\n");

            foreach (ProcessorItem item in items) {
                string srcFilepath = item.Value;
                string dstFilepath = dstTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);

                pipeline.Output.WriteLine(Utility.Level.Info, $"Copying: &-6;{srcFilepath}&-^; -> &-6;{dstFilepath}&-^;");

                if (!pipeline.Simulate) {
                    if (File.Exists(srcFilepath)) {

                        string dir = Path.GetDirectoryName(dstFilepath);
                        if (!string.IsNullOrEmpty(dir))
                            Directory.CreateDirectory(dir);

                        if (!File.Exists(dstFilepath)) {
                            File.Copy(srcFilepath, dstFilepath);
                           
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
