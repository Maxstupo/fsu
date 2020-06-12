using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Detail {
    public class PropertyProviderList : IPropertyProviderList {

        private readonly IPropertyProvider fallbackProvider;

        private readonly List<IPropertyProvider> propertyProviders = new List<IPropertyProvider>();

        public PropertyProviderList(IPropertyProvider fallbackProvider) {
            this.fallbackProvider = fallbackProvider;
        }

        public void Begin() {
            foreach (IPropertyProvider propertyProvider in propertyProviders)
                propertyProvider.Begin();
        }

        public void End() {
            foreach (IPropertyProvider propertyProvider in propertyProviders)
                propertyProvider.End();
        }

        public void Add(IPropertyProvider propertyProvider) {
            propertyProviders.Add(propertyProvider);
        }

        public void Clear() {
            propertyProviders.Clear();
        }

        public PropertyItem GetProperty(IPropertyProvider _, ProcessorItem item, string propertyName) {
            foreach (IPropertyProvider propertyProvider in propertyProviders) {
                PropertyItem property = propertyProvider.GetProperty(this, item, propertyName);
                if (property != null)
                    return property;
            }

            return fallbackProvider?.GetProperty(this, item, propertyName);
        }
    }
}
