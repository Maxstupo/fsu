namespace Maxstupo.Fsu.Core.Utility.Table {
    public interface IConsoleTableConverter {
        string ConvertCellValue(object obj, int rowIndex, int columnIndex);
    }


}
