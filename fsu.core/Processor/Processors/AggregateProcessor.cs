namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;

    public abstract class AggregateProcessor : IProcessor {

        protected abstract string Key { get; }

        private readonly string itemPropertyName;
        private readonly string outputPropertyName;

        public AggregateProcessor(string itemPropertyName, string outputPropertyName = null) {
            this.itemPropertyName = itemPropertyName ?? throw new ArgumentNullException(nameof(itemPropertyName));
            this.outputPropertyName = outputPropertyName;
        }

        protected abstract PropertyItem CalculateAggregation(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items);

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            PropertyItem result = CalculateAggregation(pipeline, items);

            if (result != null) {
                string gPropertyName = outputPropertyName ?? $"{Key}_{itemPropertyName}";

                pipeline.PropertyStore.SetProperty(gPropertyName, result);
            }

            return items;
        }

        protected PropertyItem GetProperty(IProcessorPipeline pipeline, ProcessorItem item) {
            return item.GetProperty(pipeline.PropertyProvider, itemPropertyName);
        }

        public override string ToString() {
            return $"{GetType().Name}[key={Key}, itemPropertyName={itemPropertyName}, outputPropertyName={outputPropertyName}]";
        }


    }

}