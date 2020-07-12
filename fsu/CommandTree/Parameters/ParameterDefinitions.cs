namespace Maxstupo.Fsu.CommandTree.Parameters {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class ParameterDefinitions : IEnumerable<ParamDef> {

        private readonly Dictionary<string, ParamDef> parameters = new Dictionary<string, ParamDef>();

        public ParamDef this[int index] => Get(index);
        public ParamDef this[string name] => Get(name);

        public bool IsEmpty => Count == 0;
        public int Count => parameters.Count;

        public ParameterDefinitions Add(ParamDef param) {
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (Contains(param))
                throw new ArgumentException($"Parameter name '{param.Name}' is already defined!");

            if (!param.IsOptional) {
                for (int i = 0; i < parameters.Count; i++) {
                    ParamDef def = parameters.ElementAt(i).Value;

                    if (def.IsOptional)
                        throw new ArgumentException($"{param.GetType().Name} ({param.Name}) must be optional.");
                }
            }
            parameters.Add(param.Name, param);

            return this;
        }

        public bool Contains(ParamDef param) {
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            return Contains(param.Name);
        }

        public bool Contains(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return parameters.ContainsKey(name);
        }

        public ParameterDefinitions Remove(ParamDef param) {
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            return Remove(param.Name);
        }

        public ParameterDefinitions Remove(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            parameters.Remove(name);
            return this;
        }

        public ParamDef Get(int index) {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return parameters.Values.ElementAt(index);
        }

        public ParamDef Get(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return parameters.TryGetValue(name, out ParamDef value) ? value : null;
        }

        public string GetUsage() {
            StringBuilder sb = new StringBuilder();

            foreach (ParamDef param in parameters.Values)
                sb.Append(param.Usage).Append(" ");

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public ParameterDefinitions Clear() {
            parameters.Clear();
            return this;
        }

        public IEnumerator<ParamDef> GetEnumerator() {
            return parameters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}