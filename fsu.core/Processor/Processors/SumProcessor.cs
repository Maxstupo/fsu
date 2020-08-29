namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Utility;

    public sealed class SumProcessor : AggregateProcessor {

        protected override string Key => "sum";

        public SumProcessor(string itemPropertyName, string outputPropertyName = null) : base(itemPropertyName, outputPropertyName) {

        }

        protected override PropertyItem CalculateAggregation(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            double total =0;

            Enum unit = null;

            foreach (ProcessorItem item in items) {
                PropertyItem property = GetProperty(pipeline, item);

                if (property == null)
                    continue;

                if (!property.IsNumeric) {
                    pipeline.Output.WriteLine(Level.Warn, "The sum processor can only sum numeric properties!");
                    return null;
                }

                total += property.ValueNumber;

                unit = property.Unit;
            }

            return new PropertyItem(total, unit);
        }


    }

}