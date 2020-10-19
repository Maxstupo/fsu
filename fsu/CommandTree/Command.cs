namespace Maxstupo.Fsu.CommandTree {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.CommandTree.Parameters;
    using Maxstupo.Fsu.Core.Utility;

    public delegate void CommandExecuted(CommandData data);

    public class Command : Node<Command> {

        public string Name { get; set; }

        public string Description { get; set; }

        public string Keyword { get; }

        public Aliases Aliases { get; }

        public ParameterDefinitions Parameters { get; } = new ParameterDefinitions();

        public Visibility Visibility { get; set; } = Visibility.All;

        public event CommandExecuted OnExecuted;


        public Command(string name, string keyword = null, Aliases aliases = null, string description = null, Command parent = null) : base(parent) {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Keyword = keyword?.Replace(" ", "_") ?? name.ToLower().Replace(" ", "_");
            this.Description = description ?? string.Empty;
            this.Aliases = aliases ?? Aliases.Create();

            ValidateBranch();
        }

        private void ValidateBranch() {
            if (IsRoot)
                return;

            foreach (Command child in Children) {
                if (child != this && (child.Aliases.Contains(Aliases) || child.Keyword.ToLower() == Keyword.ToLower()))
                    throw new Exception($"Invalid Commands: {Name} ({Keyword}) <--> {child.Name} ({child.Keyword})");
            }
        }

        public CommandData Execute(string[] tokens, IOutput output) {
            if (!ParameterValues.Parse(Parameters, tokens, out ParameterValues values))
                return null;

            CommandData data = new CommandData(values, output);

            OnExecuted?.Invoke(data);

            return data.DisplayHelp ? null : data;
        }

        public void GetDecendents(ref List<Command> decendents, bool ignoreVisibility = false) {
            if (ignoreVisibility || Visibility.HasFlag(Visibility.Self))
                decendents.Add(this);

            if (ignoreVisibility || Visibility.HasFlag(Visibility.Decendents)) {
                foreach (Command child in Children)
                    child.GetDecendents(ref decendents);
            }
        }

        public string GetUsage(bool includeParams = true) {
            string usage = string.Empty;
            if (includeParams) {
                usage = Parameters.GetUsage();
                if (usage.Length > 0)
                    usage += " ";
            }
            GetUsage(ref usage);
            return usage;
        }

        private void GetUsage(ref string usageStr) {
            usageStr = Keyword + " " + usageStr;
            if (!IsRoot)
                Parent.GetUsage(ref usageStr);
        }

        public virtual bool KeywordMatches(string keyword) {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));
            return keyword.ToLower() == this.Keyword.ToLower() || Aliases.Contains(keyword);
        }

        public CommandLookup Find(string[] tokens, char paramPrefix = '@') {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (tokens.Length == 0)
                return null;

            if (tokens[0].StartsWith(paramPrefix.ToString())) {
                tokens[0] = tokens[0].Substring(1);
                return null;
            } else if (!KeywordMatches(tokens[0])) {
                return null;
            }

            tokens = tokens.Skip(1).ToArray();

            foreach (Command child in Children) {
                CommandLookup result = child.Find(tokens);
                if (result != null)
                    return result;
            }

            return new CommandLookup(this, tokens);
        }


    }

}