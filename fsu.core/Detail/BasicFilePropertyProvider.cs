using Maxstupo.Fsu.Core.Processor;

namespace Maxstupo.Fsu.Core.Detail {
    public class BasicFilePropertyProvider : IPropertyProvider {
        public void Begin() {
            //    ColorConsole.WriteLine($"BasicFilePropertyProvider.Begin()");
        }

        public void End() {
            //     ColorConsole.WriteLine($"BasicFilePropertyProvider.End()");
        }

        public Property GetProperty(ProcessorItem item, string propertyName) {
            //  ColorConsole.WriteLine($"BasicFilePropertyProvider.GetProperty(\"{propertyName}\")");
            return null;
        }
    }
}
