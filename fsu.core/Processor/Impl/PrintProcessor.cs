using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using Maxstupo.Fsu.Core.Utility;
using Maxstupo.Fsu.Core.Utility.Table;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("print")]
    public class PrintProcessor : IProcessor {

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            string[] columns = null;

            ProcessorItem pi = items.FirstOrDefault();
            if (pi != null) {
                columns = new string[pi.Value.Split('|').Length + 1];
                columns.Populate(string.Empty);
            }

            if (columns != null && columns.Length > 2) {
                ConsoleTable table = new ConsoleTable {
                    UpdateMode = UpdateMode.OnWrite
                };
                table.Printer = new ColorConsoleTablePrinter();

                table.Columns.Set(columns);
                for (int i = 0; i < items.Count; i++)
                    table.Rows.Add((new string[] { i.ToString() }).Concat(items[i].Value.Split('|')).ToArray());

                table.WriteRows(false);

            } else {
                for (int i = 0; i < items.Count; i++)
                    ColorConsole.WriteLine($"&-8;{i,3}:&-^; {items[i].Value}");
            }
            return true;
        }

        public static object Construct(string name, TokenList list) {
            if (list.Count > 0)
                return $"has unexpected parameters {list.Next().Location}";
            return new PrintProcessor();
        }

    }

}
