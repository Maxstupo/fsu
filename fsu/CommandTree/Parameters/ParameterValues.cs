namespace Maxstupo.Fsu.CommandTree.Parameters {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.CommandTree.Utility;

    public sealed class ParameterValues {

        private readonly object[] objects;

        private readonly Dictionary<string, int?> lookup = new Dictionary<string, int?>();

        public int Count => objects.Length;

        public object this[int index] => Get<object>(index);
        public object this[string name] => Get<object>(name);

        private ParameterValues(int length) {
            this.objects = new object[length];
        }

        public bool IsIndexValid(int index) {
            return !(index < 0 || index >= Count);
        }

        public int GetNameIndex(string name) {
            return lookup.TryGetValue(name, out int? result) ? (result ?? -1) : -1;
        }

        public T Get<T>(string name, T defaultValue = default(T)) {
            return Get(GetNameIndex(name), defaultValue);
        }

        public T Get<T>(int index, T defaultValue = default(T)) {
            return IsIndexValid(index) ? (T) Convert.ChangeType(objects[index], typeof(T)) : defaultValue;
        }

        public static bool IsTokenValid(ParamDef def, string token) {
            if (def == null)
                throw new ArgumentNullException(nameof(def));
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (def.Type.IsAssignableFrom(typeof(float))) {
                if (!float.TryParse(token, out float result))
                    return false;

            } else if (def.Type.IsAssignableFrom(typeof(int))) {
                if (!int.TryParse(token, out int result))
                    return false;

            } else if (def.Type.IsAssignableFrom(typeof(bool))) {
                if (token.ToLower() != "true" && token.ToLower() != "false" && token != "1" && token != "0")
                    return false;

            } else if (def.Type.IsAssignableFrom(typeof(long))) {
                if (!long.TryParse(token, out long result))
                    return false;

            } else if (def.Type.IsEnum) {
                bool found = false;
                foreach (string name in def.Type.GetEnumNames()) {
                    if (name.ToLower() == token.ToLower()) {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }

            return true;
        }

        public static bool AreTokensValid(ParameterDefinitions definitions, string[] tokens) {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (tokens.Length > definitions.Count)
                return false;

            for (int i = 0; i < definitions.Count; i++) {
                ParamDef definition = definitions[i];
                string token = tokens.Get(i, null);

                if (token != null && token.Length > 0) {

                    if (!IsTokenValid(definition, token))
                        return false;

                } else if (definition.DefaultValue == null) { // Required param.
                    return false;
                }
            }

            return true;
        }

        public static object ConvertToken(ParamDef definition, string value) {
            if (definition.Type.IsAssignableFrom(typeof(float))) {
                float.TryParse(value, out float result);
                return result;
            } else if (definition.Type.IsAssignableFrom(typeof(int))) {
                int.TryParse(value, out int result);
                return result;
            } else if (definition.Type.IsAssignableFrom(typeof(bool))) {
                return (value.ToLower() == "true" || value == "1");

            } else if (definition.Type.IsAssignableFrom(typeof(long))) {
                long.TryParse(value, out long result);
                return result;
            } else if (definition.Type.IsEnum) {
                return Enum.Parse(definition.Type, value, true);
            }
            return null;
        }

        public static bool Parse(ParameterDefinitions definitions, string[] values, out ParameterValues output) {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (!AreTokensValid(definitions, values)) {
                output = null;
                return false;
            }

            ParameterValues parameters = new ParameterValues(definitions.Count);

            List<object> paramValues = new List<object>();

            for (int i = 0; i < definitions.Count; i++) {
                ParamDef definition = definitions[i];
                string value = values.Get(i, null);

                if (string.IsNullOrEmpty(value)) // Apply default value.
                    value = definition.DefaultValue?.ToString();

                if (value != null) {

                    parameters.lookup.Add(definition.Name, i);

                    object val = ConvertToken(definition, value);

                    parameters.objects[i] = val ?? value;

                } else {
                    throw new Exception("Error creating params!");
                }
            }

            output = parameters;
            return true;

        }

    }
}
