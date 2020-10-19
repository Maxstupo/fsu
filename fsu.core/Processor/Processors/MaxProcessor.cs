namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Utility;

    public sealed class MaxProcessor : AggregateProcessor {

        protected override string Key => "max";

        public MaxProcessor(string itemPropertyName, string outputPropertyName = null) : base(itemPropertyName, outputPropertyName) {

        }

        protected override PropertyItem CalculateAggregation(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            double maximum = double.MinValue;

            Enum unit = null;

            foreach (ProcessorItem item in items) {
                PropertyItem property = GetProperty(pipeline, item);

                if (property == null)
                    continue;

                if (!property.IsNumeric) {
                    pipeline.Output.WriteLine(Level.Warn, "The avg processor can only average numeric properties!");
                    return null;
                }

                maximum = Math.Max(maximum, property.ValueNumber);

                unit = property.Unit;
            }

            return new PropertyItem(maximum, unit);
        }


    }

}