namespace Maxstupo.Fsu.Core.Detail {

    public interface IPropertyStore {

        void Clear();

        void SetProperty(string propertyName, PropertyItem property);

        PropertyItem GetProperty(string propertyName);

    }

}