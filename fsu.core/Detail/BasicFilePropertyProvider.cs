using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Core.Detail {
    public class BasicFilePropertyProvider : IPropertyProvider {
        public void Begin() {
            ColorConsole.WriteLine($"BasicFilePropertyProvider.Begin()");
        }

        public void End() {
            ColorConsole.WriteLine($"BasicFilePropertyProvider.End()");
        }

        public Property GetProperty(ProcessorItem item, string propertyName) {
            ColorConsole.WriteLine($"BasicFilePropertyProvider.GetProperty(\"{propertyName}\")");
            return null;
        }
    }
}
