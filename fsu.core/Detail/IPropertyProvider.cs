using Maxstupo.Fsu.Core.Processor;

namespace Maxstupo.Fsu.Core.Detail {

    public interface IPropertyProvider {

        void Begin();

        void End();

        Property GetProperty(ProcessorItem item, string propertyName);

    }

}
