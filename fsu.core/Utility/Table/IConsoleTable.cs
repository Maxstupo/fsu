namespace Maxstupo.Fsu.Core.Utility.Table {

    public interface IConsoleTable {

        ConsoleTableColumns Columns { get; }

        ConsoleTableRows Rows { get; }

        int TableLength { get; }

        bool IsDirty { get; }

        UpdateMode UpdateMode { get; set; }


        void Update();

        void Write(bool rowDividers = true, bool headerDividerCap = true);

        void WriteDivider(ConsoleTableSection root = ConsoleTableSection.None);

        void WriteHeader(bool headerDividerCap = true);

        void WriteRows(bool writeDividers = true);

    }

}