using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Core {

    public class ProcessorPipeline : IPropertyStore {

        public FilePropertyProviderList FilePropertyProviders { get; } = new FilePropertyProviderList(null);

        private readonly List<IProcessor> processors = new List<IProcessor>();
        private readonly Dictionary<string, Property> sharedProperties = new Dictionary<string, Property>();
        private readonly bool silent;

        public ProcessorPipeline(bool silent) {
            this.silent = silent;
            FilePropertyProviders.Add(new BasicPropertyProvider());
        }


        public List<ProcessorItem> Run(params string[] items) {
            ClearStore();

            List<ProcessorItem> _items = items.Select(x => new ProcessorItem(x)).ToList();

            foreach (IProcessor processor in processors) {
                if (!silent)
                    ColorConsole.WriteLine($"&03;Executing: {processor.GetType().Name,-20} (with &-e;{_items.Count}&-^; items)&^^;");

                int count = _items.Count;

                Stopwatch sw = Stopwatch.StartNew();
                if (!processor.Process(FilePropertyProviders, this, ref _items))
                    return null;
                sw.Stop();
                double ms = sw.Elapsed.TotalMilliseconds;
                ColorConsole.WriteLine($"\t &03;Elapsed: &-e;{ms:0.##}&-^; ms (&-e;{(ms / count):0.##}&-^; ms/item)&^^;");
            }

            return _items;
        }

        public void Add(IProcessor processor) {
            processors.Add(processor);
        }

        public void Clear() {
            processors.Clear();
        }

        public void ClearStore() {
            sharedProperties.Clear();
        }

        public void SetProperty(string propertyName, Property property) {
            ColorConsole.WriteLine($"  Setting global property '&-e;{propertyName}&-^;' => '&-e;{property.Value}&-^;'");
            sharedProperties.Remove(propertyName);
            sharedProperties.Add(propertyName, property);
        }

        public Property GetProperty(string propertyName) {
            return sharedProperties.TryGetValue(propertyName, out Property property) ? property : null;
        }

    }

}
