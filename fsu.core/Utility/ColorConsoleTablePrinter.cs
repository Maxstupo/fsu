namespace Maxstupo.Fsu.Core.Utility {

    using Maxstupo.Fsu.Core.Utility.Table;
    using System;

    public class ColorConsoleTablePrinter : IConsoleTablePrinter {

        private readonly IOutput console;
        private readonly Level level;

        public ColorConsoleTablePrinter(IOutput console, Level level) {
            this.console = console ?? throw new ArgumentNullException(nameof(console));
            this.level = level;
        }

        public void Write(string str, ConsoleTableSection type, int columnIndex, int rowIndex) {
            console.Write(level, str);
        }

        public void EndTable() {
        }

        public void StartTable(ConsoleTable table) {
        }

    }

}