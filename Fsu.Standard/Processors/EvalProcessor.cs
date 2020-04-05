using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Standard.Processor {
    public class EvalProcessor : IProcessor {



        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            foreach (ProcessorItem item in items) {

                List<IProcessor> processors = pipeline.Interpreter.Eval(item.Value);

                if (processors.Count > 0) {
                    IEnumerable<ProcessorItem> result = pipeline.Process(processors);
                }

            }

            return Enumerable.Empty<ProcessorItem>();
        }



        public override string ToString() {
            return $"{nameof(EvalProcessor)}";
        }
    }
}
