using Maxstupo.Fsu.Core.Format;
using Maxstupo.Fsu.Core.Processor;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Processors {
    public class TransformProcessor : IProcessor {

        private readonly FormatTemplate template;

        public TransformProcessor(FormatTemplate template) {
            this.template = template ?? throw new ArgumentNullException(nameof(template));
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            foreach (ProcessorItem item in items) {
                item.Value = template.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);

                yield return item;
            }
        }

        public override string ToString() {
            return $"{GetType().Name}[template='{template.Template}']";
        }

    }
}
