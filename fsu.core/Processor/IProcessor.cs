using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor {

    public interface IProcessor {


        IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items);

    }

}
