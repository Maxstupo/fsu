namespace Maxstupo.Fsu.CommandTree.Parameters {
   
    using System;

    public sealed class ParamDef {

        public string Name { get; }

        public string Description { get; }

        public Type Type { get; }

        public object DefaultValue { get; }

        public bool IsOptional => DefaultValue != null;

        public string Usage => (IsOptional ? "[" : "<") + Name + (IsOptional ? "]" : ">");

        public ParamDef(string name, object defaultValue = null, Type type = null, string description = null) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Type = type ?? typeof(string);
            DefaultValue = defaultValue;

            if (DefaultValue != null && DefaultValue.GetType() != Type)
                throw new ArgumentException($"DefaultValue is type {DefaultValue.GetType().FullName}, expecting {Type.FullName}", nameof(defaultValue), null);
        }

    }

}