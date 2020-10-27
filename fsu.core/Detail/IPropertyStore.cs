namespace Maxstupo.Fsu.Core.Detail {

    using System.Collections.Generic;

    public enum Persistence {
        Runtime,
        Normal
    }

    public interface IPropertyStore : IEnumerable<KeyValuePair<string, PropertyItem>> {

        int Count { get; }


        void Clear();

        void ClearAll();

        void SetProperty(string propertyName, PropertyItem property, Persistence persistence = Persistence.Normal);

        PropertyItem GetProperty(string propertyName);

    }

}