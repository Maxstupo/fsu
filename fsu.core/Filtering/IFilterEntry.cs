namespace Maxstupo.Fsu.Core.Filtering {

    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public interface IFilterEntry {

        bool Evaluate(IConsole console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item);

    }

}