using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Utility.Table {

    public abstract class ConsoleTableCollection<T> : IEnumerable<T> {

        public T this[int i] => list[i];

        private readonly List<T> list = new List<T>();

        public int Count => list.Count;

        public event EventHandler OnChange;

        protected void NotifyOnChange() {
            OnChange?.Invoke(this, EventArgs.Empty);
        }

        public virtual ConsoleTableCollection<T> Add(T item) {
            list.Add(item);
            NotifyOnChange();
            return this;
        }

        public virtual ConsoleTableCollection<T> Insert(int index, T item) {
            list.Insert(index, item);
            NotifyOnChange();
            return this;
        }

        public virtual ConsoleTableCollection<T> Clear() {
            list.Clear();
            NotifyOnChange();
            return this;
        }

        public virtual ConsoleTableCollection<T> Remove(int index) {
            list.RemoveAt(index);
            NotifyOnChange();
            return this;
        }

        public virtual bool Remove(T item) {
            bool result = list.Remove(item);

            if (result)
                NotifyOnChange();
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator() {
            return list.GetEnumerator();
        }

    }

}
