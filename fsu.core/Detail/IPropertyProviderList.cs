namespace Maxstupo.Fsu.Core.Detail {

    public interface IPropertyProviderList : IPropertyProvider {

        void Add(IPropertyProvider propertyProvider);

        void Clear();

    }

}
