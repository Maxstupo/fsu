using System.Collections;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Detail {

    public interface IPropertyStore :IEnumerable<KeyValuePair<string, PropertyItem>>{

        int Count { get; }


   
        void Clear();

        void ClearAll();

        void SetProperty(string propertyName, PropertyItem property, bool persistent = false);

        PropertyItem GetProperty(string propertyName);

    }

}