namespace Maxstupo.Fsu.CommandTree {

    using System;

    public sealed class CommandLookup {

        public Command Command { get; }

        public string[] Tokens { get; }

        public CommandLookup(Command command, string[] tokens = null) {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.Tokens = tokens ?? new string[0];
        }

    }

}