namespace Maxstupo.Fsu.Core.Utility.Table {

    public class ConsoleTableConverter : IConsoleTableConverter {

        public string ConvertCellValue(object obj, int rowIndex, int columnIndex) {
            return obj?.ToString() ?? string.Empty;
        }

    }

}
