using Maxstupo.Fsu.Core.Detail;
using System.Collections.Generic;
using System.Reflection;

namespace Maxstupo.Fsu.Core.Processor {

    public abstract class PropertyProcessor : IProcessor {

        public string PropertyName { get; }

        public PropertyProcessor(string propertyName) {
            PropertyName = propertyName;
        }

        protected abstract double CalculateProperty(IFilePropertyProvider propertyProvider, ref List<ProcessorItem> items);

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            double result = CalculateProperty(propertyProvider, ref items);
            if (!double.IsNaN(result)) {
                Function function = GetType().GetCustomAttribute<Function>(false);
                propertyStore.SetProperty($"{function.Keyword.ToLower()}_{PropertyName}", new Property(result));
            }
            return true;
        }

        protected Property GetProperty(IFilePropertyProvider propertyProvider, ProcessorItem item) {
            return item.GetFileProperty(propertyProvider, PropertyName);
        }

    }

}
