namespace Maxstupo.Fsu.Core.Processor.Processors {
   
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using Maxstupo.Fsu.Core.Utility.Table;

    public class PrintProcessor : IProcessor {

        private readonly string[] separator;

        public PrintProcessor(string separator = null) {
            this.separator = separator == null ? null : new string[] { separator };
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            if (separator != null) {

                ConsoleTable table = new ConsoleTable {
                    Printer = new ColorConsoleTablePrinter(pipeline.Output, Level.None)
                };


                string value = items.FirstOrDefault()?.Value ?? null;
                if (value != null) {
                    int columnCount = value.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length;
                    for (int y = 0; y < columnCount; y++)
                        table.Columns.Add(string.Empty);
                } else {
                    table.Columns.Set("#", "Value");
                }


                int i = 0;
                string[] columns = new string[1];
                foreach (ProcessorItem item in items) {
                    columns[0] = (i++).ToString();
                    table.Rows.Add(columns.Concat(item.Value.Split(separator, StringSplitOptions.RemoveEmptyEntries)).ToArray());
                }
                table.WriteRows(false);

            } else {
                int i = 0;
                foreach (ProcessorItem item in items)
                    pipeline.Output.WriteLine(Level.None, $"&-8;{i++,3}:&-^; {item.Value}");
            }


            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }

    }
}
