using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Core.Processor.Standard {
    public class PrintProcessor : IProcessor {
    

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            foreach (ProcessorItem item in items)
                ColorConsole.WriteLine(item.Value);

            return items;
        }
    }
}
