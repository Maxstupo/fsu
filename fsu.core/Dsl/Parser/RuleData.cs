using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Dsl.Parser {

    public class RuleData : List<object> {
        public T Get<T>(int index) {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            object value = this[index];
            return (T) Convert.ChangeType(value, typeof(T));
        }
    }

}
