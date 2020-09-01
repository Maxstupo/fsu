namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using System;
    using System.Collections.Generic;

    public class RuleData : Dictionary<string, object> {

        public T Get<T>(string key) {
            return TryGetValue(key, out object value) ? ((T) Convert.ChangeType(value, typeof(T))) : default;
        }

    }

}