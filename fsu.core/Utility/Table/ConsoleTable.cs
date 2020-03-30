using System;
using System.Collections.Generic;
using System.Text;

namespace Maxstupo.Fsu.Core.Utility.Table {

    public enum UpdateMode {
        OnChange,
        OnWrite,
        Disabled
    }

    public class ConsoleTable : IConsoleTable {
        public char RowSeperator { get; set; } = '-';

        public char RowColumnJunction { get; set; } = '+';

        public char ColumnSeperator { get; set; } = '|';

        public int CellPadding { get; set; } = 2;

        public ConsoleTableColumns Columns { get; } = new ConsoleTableColumns();

        public ConsoleTableRows Rows { get; } = new ConsoleTableRows();

        public int TableLength { get; private set; } = 0;

        public IConsoleTablePrinter Printer { get; set; } = new ConsoleTablePrinter();

        public IConsoleTableConverter Converter { get; set; }// = new ConsoleTableConverter();

        public bool IsDirty { get; private set; }

        public UpdateMode UpdateMode { get; set; } = UpdateMode.OnChange;


        public ConsoleTable() {
            Columns.OnChange += Data_OnChange;
            Rows.OnChange += Data_OnChange;
        }

        private void Data_OnChange(object sender, EventArgs e) {
            IsDirty = true;

            if (UpdateMode == UpdateMode.OnChange)
                Update();
        }

        public void Update() {
            TableLength = 2 * Columns.Count + 1;

            for (int i = 0; i < Columns.Count; i++) {
                TableLength += UpdateColumnWidth(i);
            }

            IsDirty = false;
        }

        private int UpdateColumnWidth(int col) {
            if (col < 0 || col >= Columns.Count)
                return 0;

            ConsoleTableColumn column = Columns[col];
            column.Width = column.HeaderText.Length + 1 + CellPadding;

            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++) {
                List<object> row = Rows[rowIndex];
                if (col >= row.Count)
                    continue;
                string value = ConvertValue(row[col], rowIndex, col);

                column.Width = Math.Max(column.Width, value.Length + 1 + CellPadding);
            }

            return column.Width;
        }

        private void AttemptUpdate() {
            if (IsDirty && UpdateMode == UpdateMode.OnWrite)
                Update();
        }

        public void Write(bool rowDividers = true, bool headerDividerCap = true) {
            AttemptUpdate();

            Printer.StartTable(this);

            WriteHeader(headerDividerCap);
            WriteRows(rowDividers);

            Printer.EndTable();

        }

        public void WriteDivider(ConsoleTableSection root = ConsoleTableSection.None) {
            AttemptUpdate();

            Printer.Write($"{RowColumnJunction}", root | ConsoleTableSection.Divider | ConsoleTableSection.Seperator, -1, -1);

            for (int colIndex = 0; colIndex < Columns.Count; colIndex++) {
                ConsoleTableColumn column = Columns[colIndex];
                Printer.Write(Repeat(RowSeperator.ToString(), column.Width + 1), root | ConsoleTableSection.Divider | ConsoleTableSection.Column, colIndex, -1);
                Printer.Write($"{RowColumnJunction}", root | ConsoleTableSection.Divider | ConsoleTableSection.Seperator, -1, -1);
            }

            Printer.Write("\n", root | ConsoleTableSection.Divider, -1, -1);
        }

        public void WriteHeader(bool headerDividerCap = true) {
            AttemptUpdate();

            if (headerDividerCap)
                WriteDivider(ConsoleTableSection.Header);

            Printer.Write($"{ColumnSeperator}", ConsoleTableSection.Seperator | ConsoleTableSection.Header, -1, -1);

            for (int colIndex = 0; colIndex < Columns.Count; colIndex++) {
                ConsoleTableColumn column = Columns[colIndex];

                Printer.Write(" ", ConsoleTableSection.Padding | ConsoleTableSection.Header, colIndex, -1);
                Printer.Write(column.HeaderText, ConsoleTableSection.Column | ConsoleTableSection.Header, colIndex, -1);
                Printer.Write(Repeat(" ", column.Padding), ConsoleTableSection.Padding | ConsoleTableSection.Header, colIndex, -1);
                Printer.Write($"{ColumnSeperator}", ConsoleTableSection.Seperator | ConsoleTableSection.Header, -1, -1);
            }

            Printer.Write("\n", ConsoleTableSection.Header, -1, -1);


            WriteDivider(ConsoleTableSection.Header);
        }

        public void WriteRows(bool writeDividers = true) {
            AttemptUpdate();

            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++) {
                List<object> row = Rows[rowIndex];

                WriteRow(rowIndex, writeDividers, row.ToArray());
            }
        }

        public void WriteRow(int rowIndex, bool writeDividers = true, params object[] columnValues) {
            AttemptUpdate();

            Printer.Write($"{ColumnSeperator}", ConsoleTableSection.Row | ConsoleTableSection.Seperator, -1, rowIndex);



            for (int columnIndex = 0; columnIndex < Columns.Count; columnIndex++) {
                ConsoleTableColumn columnDef = Columns[columnIndex];

                bool hasColumnValue = columnIndex < columnValues.Length;

                object columnObject = hasColumnValue ? columnValues[columnIndex] : null;
                string columnValue = ConvertValue(columnObject, rowIndex, columnIndex);

                Printer.Write(" ", ConsoleTableSection.Row | ConsoleTableSection.Padding, columnIndex, rowIndex);
                Printer.Write(columnValue, ConsoleTableSection.Row | ConsoleTableSection.Column, columnIndex, rowIndex);
                Printer.Write(Repeat(" ", columnDef.Width - columnValue.Length), ConsoleTableSection.Row | ConsoleTableSection.Padding, columnIndex, rowIndex);
                Printer.Write($"{ColumnSeperator}", ConsoleTableSection.Row | ConsoleTableSection.Seperator, -1, rowIndex);

            }

            Printer.Write("\n", ConsoleTableSection.Row, -1, rowIndex);

            if (writeDividers)
                WriteDivider(ConsoleTableSection.Row);
        }

        protected virtual string ConvertValue(object obj, int rowIndex, int columnIndex) {
            return Converter?.ConvertCellValue(obj, rowIndex, columnIndex) ?? obj?.ToString() ?? string.Empty;
        }

        private static string Repeat(string str, int times) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < times; i++)
                sb.Append(str);
            return sb.ToString();
        }

    }

}
