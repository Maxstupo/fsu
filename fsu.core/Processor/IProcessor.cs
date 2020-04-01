using Maxstupo.Fsu.Core.Detail;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor {

    public interface IProcessor {

        bool ProcessItem(IProcessorPipeline pipeline, IPropertyProvider propertyProvider, IPropertyStore propertyStore);

    }

}
