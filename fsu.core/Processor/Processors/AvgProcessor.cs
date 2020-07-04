namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Utility;

    public sealed class AvgProcessor : IProcessor {

        private readonly string itemPropertyName;
        private readonly string outputPropertyName;

        public AvgProcessor(string itemPropertyName, string outputPropertyName = null) {
            this.itemPropertyName = itemPropertyName ?? throw new ArgumentNullException(nameof(itemPropertyName));
            this.outputPropertyName = outputPropertyName;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            double total = 0;
            int count = 0;

            Enum unit = null;

            foreach (ProcessorItem item in items) {
                PropertyItem property = item.GetProperty(pipeline.PropertyProvider, itemPropertyName);

                if (property == null)
                    continue;

                if (!property.IsNumeric) {
                    pipeline.Output.WriteLine(Level.Warn, "The avg processor can only average numeric properties!");
                    return items;
                }

                total += property.ValueNumber;
                count++;

                unit = property.Unit;
            }

            double average = total / count;

            string gPropertyName = outputPropertyName ?? $"avg_{itemPropertyName}";

            pipeline.PropertyStore.SetProperty(gPropertyName, new PropertyItem(average, unit));

            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}[itemPropertyName={itemPropertyName}, outputPropertyName={outputPropertyName}]";
        }

    }

}