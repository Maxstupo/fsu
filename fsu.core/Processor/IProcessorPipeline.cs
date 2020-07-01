namespace Maxstupo.Fsu.Core.Processor {

    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Dsl;
    using Maxstupo.Fsu.Core.Utility;
    using System.Collections.Generic;

    public interface IProcessorPipeline {

        bool Simulate { get; set; }

        IPropertyStore PropertyStore { get; }

        IPropertyProvider PropertyProvider { get; }

        IOutput Output { get; }

        IInterpreter<IProcessor> Interpreter { get; }

        IEnumerable<ProcessorItem> Process(List<IProcessor> processors);

    }

}