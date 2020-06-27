namespace Maxstupo.Fsu.Processors {

    using System.Collections.Generic;
    using System.IO;
    using Maxstupo.Fsu.Core.Processor;

    public class ScanProcessor : IProcessor {

        private readonly bool isFiles;
        private readonly SearchOption searchOption;

        public ScanProcessor(bool isFiles = true, SearchOption searchOption = SearchOption.AllDirectories) {
            this.isFiles = isFiles;
            this.searchOption = searchOption;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            foreach (ProcessorItem inputItem in items) {

                if (!Directory.Exists(inputItem.Value))
                    continue;


                IEnumerable<string> enumerable = isFiles ?
                                                         Directory.EnumerateFiles(inputItem.Value, "*", searchOption)
                                                         :
                                                         Directory.EnumerateDirectories(inputItem.Value, "*", searchOption);

                foreach (string filepath in enumerable)
                    yield return new ProcessorItem(filepath, inputItem.Value);
            }

        }

        public override string ToString() {
            return $"{GetType().Name}[isFiles={isFiles}, searchOption={searchOption}]";
        }

    }

}
