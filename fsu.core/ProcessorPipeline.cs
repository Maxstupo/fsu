using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maxstupo.Fsu.Core {

    public class ProcessorPipeline : IProcessorPipeline {

        public IPropertyProvider PropertyProvider { get; }

        public IPropertyStore PropertyStore { get; }


        public ProcessorPipeline(IPropertyProvider propertyProvider, IPropertyStore propertyStore) {
            PropertyProvider = propertyProvider ?? throw new ArgumentNullException(nameof(propertyProvider));
            PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
        }


        public IEnumerable<ProcessorItem> Process(List<IProcessor> processors, IEnumerable<ProcessorItem> items) {

            Stopwatch sw = Stopwatch.StartNew();

            foreach (IProcessor processor in processors) {

                sw.Restart();

                items = processor.Process(this, items);

                ColorConsole.WriteLine($"Done: &-b;{processor.GetType().Name}&-^; (&-a;{sw.Elapsed.TotalMilliseconds:0.#}&-^; ms)");
            }

            return items;
        }

    }

}
