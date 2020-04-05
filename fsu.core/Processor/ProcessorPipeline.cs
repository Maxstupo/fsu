using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Dsl;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maxstupo.Fsu.Core.Processor {

    public class ProcessorPipeline : IProcessorPipeline {

        public IPropertyProvider PropertyProvider { get; }

        public IPropertyStore PropertyStore { get; }

        public IDslInterpreter<IProcessor> Interpreter { get; }

        public IConsole Console { get; }


        public ProcessorPipeline(IConsole console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, IDslInterpreter<IProcessor> interpreter) {
            Console = console ?? throw new ArgumentNullException(nameof(console));
            PropertyProvider = propertyProvider ?? throw new ArgumentNullException(nameof(propertyProvider));
            PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            Interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        }


        public IEnumerable<ProcessorItem> Process(List<IProcessor> processors) {

            Stopwatch sw = Stopwatch.StartNew();

            IEnumerable<ProcessorItem> items = Enumerable.Empty<ProcessorItem>();

            foreach (IProcessor processor in processors) {

                sw.Restart();

                items = processor.Process(this, items);

                Console.WriteLine($"Done: &-b;{processor.GetType().Name}&-^; (&-a;{sw.Elapsed.TotalMilliseconds:0.#}&-^; ms)");
            }

            return items;
        }

    }

}
