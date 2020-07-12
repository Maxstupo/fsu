namespace Maxstupo.Fsu.CommandTree.Providers {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Utility;

    public abstract class HelpProvider {

        protected virtual void GenerateCommandListStart(IOutput output, MessageProvider messageProvider, int page, int ipp, int totalPages) { }

        protected abstract void GenerateCommandListing(IOutput output, MessageProvider messageProvider, Command command);

        protected virtual void GenerateCommandListEnd(IOutput output, MessageProvider messageProvider, int page, int ipp, int totalPages) { }



        public abstract void GenerateDetailedHelp(IOutput output, MessageProvider messageProvider, Command command);

        public void GenerateCommandList(IOutput output, MessageProvider messageProvider, IReadOnlyList<Command> rootCommands, int page, int ipp) {
            // Expand root commands into all command nodes. Maintain visibility of command tree.
            List<Command> allCommandNodes = new List<Command>();
            foreach (Command command in rootCommands)
                command.GetDecendents(ref allCommandNodes, false);


            if (ipp <= 0) { // All on single page.
                page = 1;
                ipp = allCommandNodes.Count;
            } else {
                page = Math.Max(Math.Abs(page), 1);
            }

            bool defaultIpp = ipp == 8;

            int totalPages = (int) Math.Ceiling((float) allCommandNodes.Count / ipp);

            GenerateCommandListStart(output, messageProvider, page, defaultIpp ? -1 : ipp, totalPages);

            foreach (Command command in allCommandNodes.Skip((page - 1) * ipp).Take(ipp))
                GenerateCommandListing(output, messageProvider, command);

            GenerateCommandListEnd(output, messageProvider, page, defaultIpp ? -1 : ipp, totalPages);
        }

    }

}