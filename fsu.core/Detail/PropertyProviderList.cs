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

        public Property GetProperty(ProcessorItem item, string propertyName) {
            foreach (IPropertyProvider propertyProvider in propertyProviders) {
                Property property = propertyProvider.GetProperty(item, propertyName);
                if (property != null)
                    return property;
            }

            return fallbackProvider?.GetProperty(item, propertyName);
        }
    }
}
