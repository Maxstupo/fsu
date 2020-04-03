using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core {

    public interface IProcessorPipeline {

        IPropertyStore PropertyStore { get; }

        IPropertyProvider PropertyProvider { get; }

        IEnumerable<ProcessorItem> Process(List<IProcessor> processors, IEnumerable<ProcessorItem> items);

    }

}
