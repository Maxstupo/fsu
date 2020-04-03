using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Core.Detail {
    public class PropertyStore : IPropertyStore {
        public void Clear() {
            ColorConsole.WriteLine($"PropertyStore.Clear()");

        }

        public Property GetProperty(string propertyName) {
            ColorConsole.WriteLine($"PropertyStore.GetProperty(\"{propertyName}\")");
            return null;
        }

        public void SetProperty(string propertyName, Property property) {
            ColorConsole.WriteLine($"PropertyStore.SetProperty(\"{propertyName}\")");

        }
    }
}
