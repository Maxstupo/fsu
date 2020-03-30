using Maxstupo.Fsu.Core.Processor;

namespace Maxstupo.Fsu.Core.Detail {

    public interface IFilePropertyProvider {

        void Begin();
        void End();

        Property GetFileProperty(ProcessorItem item, string propertyName);

    }

}
