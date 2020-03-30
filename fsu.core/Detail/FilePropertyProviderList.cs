using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Core.Detail {
    public class FilePropertyProviderList : List<IFilePropertyProvider>, IFilePropertyProvider {
        public IFilePropertyProvider FallbackProvider { get; set; }

        public FilePropertyProviderList(IFilePropertyProvider fallbackProvider) {
            FallbackProvider = fallbackProvider;
        }

        public void Begin() {
            foreach (IFilePropertyProvider propertyProvider in this)
                propertyProvider.Begin();
        }

        public void End() {
            foreach (IFilePropertyProvider propertyProvider in this)
                propertyProvider.End();
        }

        public Property GetFileProperty(ProcessorItem item, string propertyName) {
#if DEBUG
            ColorConsole.WriteLine($"  #GetFileProperty &0e;{propertyName}&^^; for file &0e;{item.Value}&^^;");
#endif
            foreach (IFilePropertyProvider propertyProvider in this) {
                Property property = propertyProvider.GetFileProperty(item, propertyName);
                if (property != null)
                    return property;
            }

            return FallbackProvider?.GetFileProperty(item, propertyName);
        }
    }

}
