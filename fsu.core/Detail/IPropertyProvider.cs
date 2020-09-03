namespace Maxstupo.Fsu.Core.Detail {

    using Maxstupo.Fsu.Core.Processor;

    public interface IPropertyProvider {

        void Begin();

        void End();

        PropertyItem GetProperty(IPropertyProvider providerList, ProcessorItem item, string propertyName);

    }

}