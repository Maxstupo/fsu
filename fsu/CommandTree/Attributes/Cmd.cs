namespace Maxstupo.Fsu.CommandTree.Attributes {

    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class Cmd : Attribute {

        public string Name { get; }

        public string Keyword { get; }

        public string Description { get; set; }

        public Cmd(string name, string keyword = null) {
            Name = name;
            Keyword = keyword;
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CmdAliases : Attribute {

        public string[] Aliases { get; }

        public CmdAliases(params string[] aliases) {
            Aliases = aliases ?? new string[0];
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CmdVisibility : Attribute {

        public Visibility Visibility { get; }

        public CmdVisibility(Visibility visibility) {
            Visibility = visibility;
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class CmdParam : Attribute {

        public string Name { get; }
        public Type Type { get; }

        public object DefaultValue { get; set; }
        public string Description { get; set; }

        public CmdParam(string name, Type type = null) {
            Name = name;
            Type = type;
        }

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CmdInclude : Attribute { }

}