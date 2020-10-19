namespace Maxstupo.Fsu.Core.Utility.Table {

    public interface IConsoleTablePrinter {

        void StartTable(ConsoleTable table);
        void EndTable();

        void Write(string str, ConsoleTableSection type, int columnIndex, int rowIndex);
        // void WriteLine(string line, ConsoleTableSection type, int columnIndex, int rowIndex);

    }

}