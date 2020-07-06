namespace Maxstupo.Fsu.Core.Processor {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Dsl;
    using Maxstupo.Fsu.Core.Utility;

    public class ProcessorPipeline : IProcessorPipeline {

        public bool Simulate { get; set; }

        public IPropertyProvider PropertyProvider { get; }

        public IPropertyStore PropertyStore { get; }

        public IInterpreter<IProcessor> Interpreter { get; }

        public IOutput Output { get; }


        public ProcessorPipeline(IOutput output, IPropertyProvider propertyProvider, IPropertyStore propertyStore, IInterpreter<IProcessor> interpreter) {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            PropertyProvider = propertyProvider ?? throw new ArgumentNullException(nameof(propertyProvider));
            PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            Interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        }


        public IEnumerable<ProcessorItem> Process(List<IProcessor> processors) {
            
            IEnumerable<ProcessorItem> items = Enumerable.Empty<ProcessorItem>();

            foreach (IProcessor processor in processors) {
                Output.WriteLine(Level.Debug, $"&03;Executing: {processor.GetType().Name,-20}&^^;");

                items = processor.Process(this, items);
            }

            return items;
        }

    }

}