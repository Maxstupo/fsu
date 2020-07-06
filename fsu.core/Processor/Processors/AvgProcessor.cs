namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Utility;

    public sealed class AvgProcessor : AggregateProcessor {

        protected override string Key => "avg";

        public AvgProcessor(string itemPropertyName, string outputPropertyName = null) : base(itemPropertyName, outputPropertyName) {

        }

        protected override PropertyItem CalculateAggregation(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            double total = 0;
            int count = 0;

            Enum unit = null;

            foreach (ProcessorItem item in items) {
                PropertyItem property = GetProperty(pipeline, item);

                if (property == null)
                    continue;

                if (!property.IsNumeric) {
                    pipeline.Output.WriteLine(Level.Warn, "The avg processor can only average numeric properties!");
                    return null;
                }

                total += property.ValueNumber;
                count++;

                unit = property.Unit;
            }

            double average = total / count;

            return new PropertyItem(average, unit);
        }


    }

}