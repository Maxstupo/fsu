namespace Maxstupo.Fsu.Processors {

    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using Maxstupo.Fsu.Core.Utility.Table;

    public class PrintProcessor : IProcessor {

        private readonly bool useTable = true;
        private readonly char separator = '|';

        public PrintProcessor(bool useTable, char separator) {
            this.useTable = useTable;
            this.separator = separator;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            if (useTable) {

                ConsoleTable table = new ConsoleTable {
                    Printer = new ColorConsoleTablePrinter(pipeline.Console)
                };


                string value = items.FirstOrDefault()?.Value ?? null;
                if (value != null) {
                    int columnCount = value.Split(separator).Length;
                    for (int y = 0; y < columnCount; y++)
                        table.Columns.Add(string.Empty);
                } else {
                    table.Columns.Set("#", "Value");
                }


                int i = 0;
                string[] columns = new string[1];
                foreach (ProcessorItem item in items) {
                    columns[0] = (i++).ToString();
                    table.Rows.Add(columns.Concat(item.Value.Split(separator)).ToArray());
                }
                table.WriteRows(false);

            } else {
                int i = 0;
                foreach (ProcessorItem item in items)
                    pipeline.Console.WriteLine($"&-8;{i++,3}:&-^; {item.Value}");
            }


            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }
}
