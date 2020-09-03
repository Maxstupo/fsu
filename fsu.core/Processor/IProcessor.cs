namespace Maxstupo.Fsu.Core.Processor {

    using System.Collections.Generic;

    public interface IProcessor {

        IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items);

    }

}