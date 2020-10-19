namespace Maxstupo.Fsu.Core.Utility.Table {

    using System.Collections.Generic;
    using System.Linq;

    public class ConsoleTableRows : ConsoleTableCollection<List<object>> {

        public ConsoleTableCollection<List<object>> Add(params object[] columnValues) {
            base.Add(columnValues.ToList());
            return this;
        }

    }
    
}