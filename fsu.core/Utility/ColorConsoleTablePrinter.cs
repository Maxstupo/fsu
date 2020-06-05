using Maxstupo.Fsu.Core.Utility.Table;
using System;

namespace Maxstupo.Fsu.Core.Utility {
    public class ColorConsoleTablePrinter : IConsoleTablePrinter {

        private readonly IConsole console;

        public ColorConsoleTablePrinter(IConsole console) {
            this.console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void Write(string str, ConsoleTableSection type, int columnIndex, int rowIndex) {
            console.Write(str);
        }

        public void EndTable() {
        }

        public void StartTable(ConsoleTable table) {
        }

    }
}
