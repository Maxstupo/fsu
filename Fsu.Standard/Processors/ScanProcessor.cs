using Maxstupo.Fsu.Core.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Maxstupo.Fsu.Standard.Processor {
    public class ScanProcessor : IProcessor {

        private readonly bool isFiles;
        private readonly SearchOption searchOption;

        public ScanProcessor(bool isFiles = true, SearchOption searchOption = SearchOption.AllDirectories) {
            this.isFiles = isFiles;
            this.searchOption = searchOption;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            IEnumerable<ProcessorItem> result = Enumerable.Empty<ProcessorItem>();

            foreach (ProcessorItem item in items) {

                if (!Directory.Exists(item.Value))
                    continue;


                IEnumerable<string> enumerable = isFiles ?
                                                         Directory.EnumerateFiles(item.Value, "*", searchOption)
                                                         :
                                                         Directory.EnumerateDirectories(item.Value, "*", searchOption);

                result = result.Concat(enumerable.Select(x => new ProcessorItem(x)));
            }

            return result;
        }

        public override string ToString() {
            return $"{GetType().Name}[isFiles={isFiles}, searchOption={searchOption}]";
        }

    }
}
