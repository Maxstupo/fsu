namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using System;
    using System.Collections.Generic;

    public class RuleData : List<object> {
     
        public T Get<T>(int index) {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            object value = this[index];
            return (T) Convert.ChangeType(value, typeof(T));
        }
 
    }

}