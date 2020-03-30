using System;

namespace Maxstupo.Fsu.Core.Utility.Table {

    public class ConsoleTableColumns : ConsoleTableCollection<ConsoleTableColumn> {

        public ConsoleTableColumns Set(params string[] headers) {
            Clear();
            foreach (string header in headers)
                Add(header);
            return this;
        }

        public ConsoleTableCollection<ConsoleTableColumn> Add(string headerText, int width = 0) {
            return Add(new ConsoleTableColumn(headerText, width));
        }

        public ConsoleTableCollection<ConsoleTableColumn> Insert(int index, string headerText) {
            return Insert(index, new ConsoleTableColumn(headerText));
        }

    }

    public class ConsoleTableColumn {

        public string HeaderText { get; }

        public int Width { get; set; }

        public int Padding => Math.Max(0, Width - HeaderText.Length);

        public ConsoleTableColumn(string headerText, int width = 0) {
            HeaderText = headerText ?? throw new ArgumentNullException(nameof(headerText));
            Width = width;
        }
    }

}
