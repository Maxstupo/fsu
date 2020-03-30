using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;

namespace Maxstupo.Fsu.Core.Filtering {

    public interface IFilterEntry {

        bool Evaluate(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item);

    }

}
