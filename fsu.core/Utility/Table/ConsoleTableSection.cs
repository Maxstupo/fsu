using System;

namespace Maxstupo.Fsu.Core.Utility.Table {

    [Flags]
    public enum ConsoleTableSection {
        None = 0,
        Row = 1,
        Column = 2,
        Seperator = 4,
        Header = 8,
        Padding = 16,
        Divider = 32,
    }

}
