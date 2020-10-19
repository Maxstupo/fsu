namespace Maxstupo.Fsu.CommandTree.Providers {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.CommandTree.Parameters;
    using Maxstupo.Fsu.CommandTree.Utility;
    using Maxstupo.Fsu.Core.Utility;
    using Maxstupo.Fsu.Core.Utility.Table;

    public class HelpTablePrinter : IConsoleTablePrinter {
        private readonly IOutput output;

        public HelpTablePrinter(IOutput output) {
            this.output = output;
        }

        public void EndTable() {
        }

        public void StartTable(ConsoleTable table) {
        }

        public void Write(string str, ConsoleTableSection type, int columnIndex, int rowIndex) {
            if (!type.HasFlag(ConsoleTableSection.Row | ConsoleTableSection.Seperator)) {
                output.Write(Level.None, str);
            }
        }

    }

    public class BasicHelpProvider : HelpProvider {

        private ConsoleTable table;

        public BasicHelpProvider() {

        }

        private void SetupTable(IOutput output) {
            if (table == null) {
                table = new ConsoleTable() {
                    Printer = new HelpTablePrinter(output),
                    UpdateMode = UpdateMode.Disabled,
                    CellPadding = 0,
                };
                table.Columns.Set("Command", "Description");
            } else {
                table.Rows.Clear();
            }
        }

        protected override void GenerateCommandListStart(IOutput output, MessageProvider messageProvider, int page, int ipp, int totalPages) {
            SetupTable(output);
        }

        protected override void GenerateCommandListing(IOutput output, MessageProvider messageProvider, Command command) {
            table.Rows.Add(command.GetUsage(), command.Description);
        }

        protected override void GenerateCommandListEnd(IOutput output, MessageProvider messageProvider, int page, int ipp, int totalPages) {
            table.Update();

            string helpTitleRaw = messageProvider.GetFormatted(StandardMessages.HelpTitle, string.Empty, page, ipp, totalPages);

            string divider = messageProvider.GetFormatted(StandardMessages.TitleDivider, page, ipp, totalPages);
            int len = (table.TableLength / 2) - (helpTitleRaw.Length / 2);
            string dividerExpanded = divider.Repeat(len);

            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.HelpTitle, dividerExpanded, page, ipp, totalPages));

            table.WriteRows(false);

            if (totalPages > 1)
                output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.CurrentHelpPage, page, totalPages));

            if (page < totalPages)
                output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.NextHelpPageTip, page + 1, (ipp == -1 ? string.Empty : ipp.ToString())));

            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpTip, page, ipp, totalPages));
        }


        public override void GenerateDetailedHelp(IOutput output, MessageProvider messageProvider, Command command) {

            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpTitle, command.Name, command.Keyword));

            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpName, command.Name), true);
            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpDescription, command.Description), true);
            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpKeyword, command.Keyword));

            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpAliases, command.Aliases.Delimited), true);

            output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpUsage, command.GetUsage()));

            if (!command.Parameters.IsEmpty) {
                output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpParamTitle), true);

                for (int i = 0; i < command.Parameters.Count; i++) {
                    ParamDef paramDef = command.Parameters[i];

                    string validValues = string.Empty;

                    if (paramDef.Type.IsEnum)
                        validValues = string.Join("|", Enum.GetNames(paramDef.Type));

                    output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpParam, paramDef.Usage, paramDef.Description, validValues));


                }
            }

            List<Command> decendents = new List<Command>();
            command.GetDecendents(ref decendents, false);
            decendents.RemoveAt(0);

            if (decendents.Count > 0) {
                output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpSubCommandTitle), true);

                foreach (Command c in decendents)
                    output.WriteLine(Level.None, messageProvider.GetFormatted(StandardMessages.DetailedHelpSubCommand, c.GetUsage(), c.Description));
            }
        }

    }

}