namespace Maxstupo.Fsu.CommandTree {

    using System;
    using Maxstupo.Fsu.CommandTree.Parameters;
    using Maxstupo.Fsu.Core.Utility;

    public sealed class CommandData {

        public bool DisplayHelp { get; set; } = false;

        public ParameterValues Parameters { get; }

        public IOutput Output { get; }

        public CommandData(ParameterValues parameters, IOutput output) {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Output = output;
        }

    }

}