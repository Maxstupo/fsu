using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Core.Utility.Table {

    public class ConsoleTableRows : ConsoleTableCollection<List<object>> {

        public ConsoleTableCollection<List<object>> Add(params object[] columnValues) {
            base.Add(columnValues.ToList());
            return this;
        }

    }

}
