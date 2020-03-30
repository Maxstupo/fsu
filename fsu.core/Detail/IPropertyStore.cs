namespace Maxstupo.Fsu.Core.Detail {

    public interface IPropertyStore {

        void ClearStore();

        void SetProperty(string propertyName, Property property);

        Property GetProperty(string propertyName);
    
    }

}
