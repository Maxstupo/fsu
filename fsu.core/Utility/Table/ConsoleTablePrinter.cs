namespace Maxstupo.Fsu.Core.Utility.Table {

    using System;

    public class ConsoleTablePrinter : IConsoleTablePrinter {

        public void Write(string str, ConsoleTableSection type, int columnIndex, int rowIndex) {
            Console.Write(str);
        }

        

        public void StartTable(ConsoleTable table) {

        }

        public void EndTable() {
        }
 
    }

}