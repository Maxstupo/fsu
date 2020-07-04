namespace Maxstupo.Fsu.Core.Detail {

    using System;
    using System.Collections.Generic;

    public class PropertyStore : IPropertyStore {

        private readonly Dictionary<string, PropertyItem> items = new Dictionary<string, PropertyItem>(StringComparer.InvariantCultureIgnoreCase);

        public void Clear() {
            items.Clear();
        }

        public PropertyItem GetProperty(string propertyName) {
            return items.TryGetValue(propertyName, out PropertyItem item) ? item : null;
        }

        public void SetProperty(string propertyName, PropertyItem property) {

            if (items.ContainsKey(propertyName))
                return;

            items.Add(propertyName, property);

        }

    }

}