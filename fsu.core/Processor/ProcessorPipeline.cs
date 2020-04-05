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

            IEnumerable<ProcessorItem> items = Enumerable.Empty<ProcessorItem>();

            Stopwatch sw = Stopwatch.StartNew();

            // TEMP
            int index = 0;


            foreach (IProcessor processor in processors) {
                sw.Restart();

                int itemCount = 0; //items.Count(); // This might negate the advantage of using enumerable.
                if (itemCount == 0) {
                    Console.WriteLine($"&03;{index}. Executing: {processor.GetType().Name,-20}&^^;");
                } else {
                    Console.WriteLine($"&03;{index}. Executing: {processor.GetType().Name,-20} (with &-e;{itemCount}&-^; items)&^^;");
                }

                items = processor.Process(this, items);

                double ms = sw.Elapsed.TotalMilliseconds;
                if (itemCount == 0) {
                    Console.WriteLine($"\t &03;{index}. Elapsed: &-e;{ms:0.###}&-^; ms&^^;");
                } else {
                    Console.WriteLine($"\t &03;{index}. Elapsed: &-e;{ms:0.###}&-^; ms (&-e;{(ms / itemCount):0.##}&-^; ms/item)&^^;");
                }
                index++;
            }

            return items;
        }

    }

}
