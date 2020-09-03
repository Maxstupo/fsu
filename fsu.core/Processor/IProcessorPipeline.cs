namespace Maxstupo.Fsu.Core.Processor {

    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Dsl;

    public interface IProcessorPipeline {

        bool Simulate { get; set; }

        IPropertyStore PropertyStore { get; }

        IPropertyProvider PropertyProvider { get; }

        IInterpreter<IProcessor> Interpreter { get; }

        IEnumerable<ProcessorItem> Process(List<IProcessor> processors);

    }

}