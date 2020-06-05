using Maxstupo.Fsu.Core.Processor;

namespace Maxstupo.Fsu.Core.Detail {

    public interface IPropertyProvider {

        void Begin();

        void End();

        // providerList is a IPropertyProviderList, we aren't using that type as we don't need access to the methods it provides.
        // aka the root provider for properties, useful for getting properties from other providers when providing composite properties.
        PropertyItem GetProperty(IPropertyProvider providerList, ProcessorItem item, string propertyName);

    }

}
