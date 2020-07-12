namespace Maxstupo.Fsu.CommandTree.Attributes {

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Maxstupo.Fsu.CommandTree.Parameters;
    using Maxstupo.Fsu.CommandTree.Utility;

    public class CommandTreeBuilder {

        private CommandTreeBuilder() { }

        private static Aliases GetAliases(Type type) {
            return Aliases.Create(type.GetAttribute<CmdAliases>()?.Aliases ?? new string[0]);
        }

        private static Aliases GetAliases(MethodInfo method) {
            return Aliases.Create(method.GetAttribute<CmdAliases>()?.Aliases ?? new string[0]);
        }

        private static void BuildSubcommandNodes(CommandInterpreter cli, ICommandNode node, Command parent) {
            if (cli == null)
                throw new ArgumentNullException(nameof(cli));
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (PropertyInfo property in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                if (property.GetCustomAttribute(typeof(CmdInclude), false) == null)
                    continue;

                if (property.GetValue(node) is ICommandNode subnode)
                    Build(cli, subnode, parent, false);
            }
        }

        public static void Build(CommandInterpreter cli, ICommandNode node, Command parent = null) {
            Build(cli, node, parent, true);
        }

        private static void Build(CommandInterpreter cli, ICommandNode node, Command parent = null, bool check = true) {
            if (cli == null)
                throw new ArgumentNullException(nameof(cli));
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            Type type = node.GetType();
            Cmd rootInfo = type.GetAttribute<Cmd>();
            if (rootInfo == null) {
                if (check)
                    throw new ArgumentException($"{node.GetType().Name} must have the attribute {typeof(Cmd).Name} defined!");
                return;
            }

            Command rootCommand = new Command(rootInfo.Name, rootInfo.Keyword, GetAliases(type), rootInfo.Description, parent);
            AddParams(rootCommand, type);
            rootCommand.OnExecuted += node.Execute;

            if (parent == null)
                cli.Register(rootCommand);

            BuildSubcommandNodes(cli, node, rootCommand);

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)) {
                Cmd cmd = method.GetAttribute<Cmd>();

                if (cmd == null)
                    continue;
                if (!HasValidParameters(method))
                    throw new Exception($"Method has {typeof(Cmd).Name} attribute, but has invalid parameters!");

                Command subcommand = new Command(cmd.Name, cmd.Keyword, GetAliases(method), cmd.Description, rootCommand);
                AddParams(subcommand, method);
                subcommand.OnExecuted += (data) => { // TODO: Replace lambda with wrapper.
                    method.Invoke(node, new object[] { data });
                };
            }


        }

        private static void AddParams(Command command, Type type) {
            if (type == null) throw new ArgumentNullException(nameof(type));
            AddParams(command, type.GetAttributes<CmdParam>());
        }

        private static void AddParams(Command command, MethodInfo info) {
            if (info == null) throw new ArgumentNullException(nameof(info));
            AddParams(command, info.GetAttributes<CmdParam>());
        }

        private static void AddParams(Command command, List<CmdParam> parameters) {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            foreach (CmdParam param in parameters)
                command.Parameters.Add(new ParamDef(param.Name, param.DefaultValue, param.Type, param.Description));
        }

        private static bool HasValidParameters(MethodInfo info) {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            ParameterInfo[] prms = info.GetParameters();
            if (prms.Length != 1)
                return false;
            return typeof(CommandData) == prms[0].ParameterType;
        }

    }

}