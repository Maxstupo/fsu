namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;

    public class ExtractProcessor : IProcessor {

        private readonly string key;
        private readonly string property;
        private readonly string regexText;

        private readonly Regex regex;


        public ExtractProcessor(string key, string property, string regex) {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.property = property ?? throw new ArgumentNullException(nameof(property));
            this.regexText = regex ?? throw new ArgumentNullException(nameof(regex));

            this.regex = new Regex(regex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            foreach (ProcessorItem item in items) {
                string propertyValue = item.GetProperty(pipeline.PropertyProvider, property).ValueText;

                Match match = regex.Match(propertyValue);


                if (!match.Success)
                    continue;


                item.TryCachePropertyValue(key, new PropertyItem(match.Value));

                if (match.Groups.Count > 1) {
                    for (int i = 1; i < match.Groups.Count; i++) {
                        Group group = match.Groups[i];
                        if (!group.Success)
                            continue;
                        item.TryCachePropertyValue($"{key}{i - 1}", new PropertyItem(group.Value));
                    }
                }

                yield return item;
            }

        }

        public override string ToString() {
            return $"{GetType().Name}[regex={regexText}, property={property}, key={key}]";
        }

    }
}
