using Maxstupo.Fsu.Core.Utility.Table;

namespace Maxstupo.Fsu.Core.Utility {

    public class ColorConsoleTablePrinter : IConsoleTablePrinter {

        public void Write(string str, ConsoleTableSection type, int columnIndex, int rowIndex) {
            ColorConsole.Write(str);
        }

        public void EndTable() { }

        public void StartTable(ConsoleTable table) { }

    }

}
