namespace Maxstupo.Fsu.CommandTree {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.CommandTree.Attributes;
    using Maxstupo.Fsu.CommandTree.Parameters;
    using Maxstupo.Fsu.CommandTree.Providers;
    using Maxstupo.Fsu.CommandTree.Utility;
    using Maxstupo.Fsu.Core.Utility;

    public class CommandInterpreter {

        private readonly List<Command> commands = new List<Command>();
        public IReadOnlyList<Command> Commands => commands.AsReadOnly();

        public IOutput Printer { get; }

        public HelpProvider HelpProvider { get; set; } = new BasicHelpProvider();
        public MessageProvider MessageProvider { get; set; } = new DictionaryMessageProvider();

        public IStringSplitter Splitter { get; set; } = new BasicStringSplitter();

        public char DetailedHelpToken { get; set; } = '?';

        public CommandInterpreter(IOutput printer, bool registerHelp = true) {
            this.Printer = printer;
            if (registerHelp)
                RegisterHelp();
        }

        public Command RegisterHelp() {
            return RegisterHelp("Help", "help", "Provides help information about commands.", DetailedHelpToken.ToString());
        }

        public Command RegisterHelp(string name, string keyword, string description, params string[] aliases) {
            Command help = new Command(name, keyword, Aliases.Create(aliases), description);
            help.Parameters.Add(new ParamDef("page", 0, typeof(int), "The help page to display."));
            help.Parameters.Add(new ParamDef("ipp", 8, typeof(int), "The number of items per page."));
            help.OnExecuted += (data) => {
                int page = data.Parameters.Get<int>("page");
                int ipp = data.Parameters.Get<int>("ipp");
                PrintCommandList(page, ipp);
            };
            Register(help);
            return help;
        }

        public void PrintCommandList(int page = 0, int ipp = -1) {
            HelpProvider.GenerateCommandList(Printer, MessageProvider, commands.AsReadOnly(), page, ipp);
        }

        public bool Process(string line, char delimiter = ' ', char group = '"') {
            string[] tokens = Splitter?.Split(line, delimiter, group) ?? line.Split(delimiter);
            return Process(tokens);
        }

        public virtual bool Process(string[] tokens) {
            if (tokens == null || tokens.Length == 0)
                return false;

            CommandLookup lookup = Find(tokens);
            if (lookup != null) {
                Command command = lookup.Command;

                if (lookup.Tokens.Length > 0 && lookup.Tokens[0] == DetailedHelpToken.ToString()) {
                    HelpProvider.GenerateDetailedHelp(Printer, MessageProvider, command);
                    return true;
                } else {
                    CommandData data = command.Execute(lookup.Tokens, Printer);
                    if (data == null) // TODO: allow specific command error messages.
                        HelpProvider.GenerateDetailedHelp(Printer, MessageProvider, command);

                    return data != null;
                }
            } else {

                string msg = MessageProvider.GetFormatted(StandardMessages.NoCommand, string.Join(" ", tokens));
                Printer.WriteLine(Level.None,msg);
            }

            return false;
        }

        public CommandLookup Find(string[] tokens) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));
            if (tokens.Length == 0)
                return null;

            return Get(tokens[0])?.Find(tokens);
        }

        public Command Get(string keyword) {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));
            return commands.Find(cmd => cmd.KeywordMatches(keyword));
        }

        public void Build(ICommandNode node, Command parent = null) {
            CommandTreeBuilder.Build(this, node, parent);
        }

        public void Register(Command command) {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (!command.IsRoot)
                throw new ArgumentException($"Command '{command.Name} ({command.Keyword})' is not the root command!");

            if (Get(command.Keyword) != null)
                throw new ArgumentException($"Command '{command.Name} ({command.Keyword})' is already registered!");


            foreach (Command rootCommand in commands) {
                if (rootCommand.Aliases.Contains(command.Aliases))
                    throw new ArgumentException($"Command '{command.Name} ({command.Keyword})' contains conflicting aliases with already registered commands!");
            }

            commands.Add(command);
        }

        public bool Unregister(string cmd) {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            Command command = Find(cmd.Split(' '))?.Command;
            if (command == null)
                return false;

            Command parent = (Command) command.Root;
            return Unregister(parent);
        }

        public bool Unregister(Command command) {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            return commands.Remove(command);
        }

    }

}