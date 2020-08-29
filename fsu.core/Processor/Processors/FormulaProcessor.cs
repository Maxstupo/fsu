namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Filtering;
    using Maxstupo.Fsu.Core.Processor;

    public class FormulaProcessor : IProcessor {

        private readonly string formula;

        public FormulaProcessor(string formula) {
            this.formula = formula; 
            // set @{ep} to @{ep} + 51
            // set <formula>
            // set <prop> to <formula>
        }
        
        // scan files top >> extract "S(\d\d)E(\d\d)" from @{name} as @{ep} >> set @{ep1} + 51 >> rename "Black Clover - Season @{ep0} - Episode @{ep1}.@{ext}"

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }
}
