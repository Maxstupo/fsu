namespace Maxstupo.Fsu.Core.Detail {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PropertyStore : IPropertyStore {

        private readonly Dictionary<string, object[]> items = new Dictionary<string, object[]>(StringComparer.InvariantCultureIgnoreCase);

        public void Clear() {
            List<string> keysToRemove = items.Where(x => !(bool) x.Value[1]).Select(x => x.Key).ToList();
            foreach (string key in keysToRemove)
                items.Remove(key);
        }

        public void ClearAll() {
            items.Clear();
        }

        public PropertyItem GetProperty(string propertyName) {
            return items.TryGetValue(propertyName, out object[] item) ? (PropertyItem) item[0] : null;
        }

        public void SetProperty(string propertyName, PropertyItem property, bool persistent = false) {

            if (items.ContainsKey(propertyName))
                return;

            items.Add(propertyName, new object[] {
               property,
               persistent
            });

        }

    }

}