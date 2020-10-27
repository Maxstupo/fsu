namespace Maxstupo.Fsu.Core.Detail {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class PropertyStore : IPropertyStore {

        private readonly Dictionary<string, object[]> items = new Dictionary<string, object[]>(StringComparer.InvariantCultureIgnoreCase);

        public int Count => items.Count;


        /// <summary>
        /// Clears all normal persistence properties from this store.
        /// </summary>
        public void Clear() {
            foreach (KeyValuePair<string, object[]> pair in items.Where(x => (Persistence) x.Value[1] == Persistence.Normal).ToList())
                items.Remove(pair.Key);
        }

        /// <summary>
        /// Clears all properties but runtime properties from this store.
        /// </summary>
        public void ClearAll() {
            foreach (KeyValuePair<string, object[]> pair in items.Where(x => (Persistence) x.Value[1] != Persistence.Runtime).ToList())
                items.Remove(pair.Key);
        }

        public IEnumerator<KeyValuePair<string, PropertyItem>> GetEnumerator() {
            foreach (KeyValuePair<string, object[]> pair in items)
                yield return new KeyValuePair<string, PropertyItem>(pair.Key, (PropertyItem) pair.Value[0]);
        }


        public PropertyItem GetProperty(string propertyName) {
            return items.TryGetValue(propertyName, out object[] item) ? (PropertyItem) item[0] : null;
        }

        public void SetProperty(string propertyName, PropertyItem property, Persistence persistence = Persistence.Normal) {

            if (items.ContainsKey(propertyName))
                return;

            items.Add(propertyName, new object[] {
               property,
               persistence
            });

        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

}