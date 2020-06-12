using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Dsl;
using Maxstupo.Fsu.Core.Utility;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor {

    public interface IProcessorPipeline {

        IPropertyStore PropertyStore { get; }

        IPropertyProvider PropertyProvider { get; }

        IConsole Console { get; }

        IDslInterpreter<IProcessor> Interpreter { get; }

        IEnumerable<ProcessorItem> Process(List<IProcessor> processors);

    }

}
