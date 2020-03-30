using Maxstupo.Fsu.Core.Detail;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor {

    public interface IProcessor {

        bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items);

    }

}
