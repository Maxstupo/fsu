namespace Maxstupo.Fsu.Core.Detail {

    public interface IPropertyStore {

        void Clear();
        
        void ClearAll();

        void SetProperty(string propertyName, PropertyItem property, bool persistent = false);

        PropertyItem GetProperty(string propertyName);
      
    }

}