namespace Maxstupo.Fsu.Standard.Processors {

    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Processor;

    public class EvalProcessor : IProcessor {

        private readonly bool joinInput = false;
        private readonly bool appendOutput = false;

        public EvalProcessor(bool joinInput, bool appendOutput) {
            this.joinInput = joinInput;
            this.appendOutput = appendOutput;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            IEnumerable<ProcessorItem> result = appendOutput ? items : Enumerable.Empty<ProcessorItem>();

            if (joinInput) {
                string value = string.Join(" ", items.Select(x => x.Value));

                result = result.Concat(Eval(pipeline, value));

            } else {
                foreach (ProcessorItem item in items)
                    result = result.Concat(Eval(pipeline, item.Value));

            }

            return result;
        }

        private IEnumerable<ProcessorItem> Eval(IProcessorPipeline pipeline, string value) {
            List<IProcessor> processors = pipeline.Interpreter.Eval(value);

            if (processors.Count > 0)
                return pipeline.Process(processors);
            return Enumerable.Empty<ProcessorItem>();
        }


        public override string ToString() {
            return $"{GetType().Name}[joinInput={joinInput}, appendOutput={appendOutput}]";
        }
    }
}
