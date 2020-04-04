﻿using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor.Standard {
    public class PrintProcessor : IProcessor {

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            foreach (ProcessorItem item in items)
                pipeline.Console.WriteLine(item.Value);

            return items;
        }
    }
}
