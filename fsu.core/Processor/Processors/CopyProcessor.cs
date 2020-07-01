namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System.Collections.Generic;
    using System.IO;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;

    public class CopyProcessor : IProcessor {

        private readonly FormatTemplate dstTemplate;
        private readonly FormatTemplate srcTemplate;

        public CopyProcessor(FormatTemplate dstTemplate, FormatTemplate srcTemplate) {
            this.dstTemplate = dstTemplate;
            this.srcTemplate = srcTemplate;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            foreach (ProcessorItem item in items) {

                string src = srcTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);
                src = Path.GetFullPath(src);

                if (!File.Exists(src)) {
                    pipeline.Console.WriteLine($"Src file doesnt exist: \"{src}\"");
                    continue;
                }

                string dst = dstTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);
                dst = Path.GetFullPath(dst);

                if (File.Exists(dst)) {
                    pipeline.Console.WriteLine($"Dst file exists: \"{dst}\"");
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(dst));
                pipeline.Console.WriteLine($"Copying \"{src}\" -> \"{dst}\"");
                File.Copy(src, dst);

            }

            return items;
        }

        public override string ToString() { 
            return $"{GetType().Name}[srcTemplate={srcTemplate}, dstTemplate={dstTemplate}]";
        }

    }
}
