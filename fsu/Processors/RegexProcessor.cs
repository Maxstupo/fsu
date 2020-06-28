namespace Maxstupo.Fsu.Processors {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;

    public class RegexProcessor : IProcessor {


        private readonly Regex regex;
        private readonly string template;
        private readonly string propertyName;


        public RegexProcessor(string regex, string template, string propertyName) {
            this.regex = new Regex(regex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
            this.template = template;
            this.propertyName = propertyName;
        }


        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            foreach (ProcessorItem item in items) {
                Match match = regex.Match(item.Value);

                if (match.Success) {
                    string value = string.Format(template, match.Groups.Cast<Group>().Select(x => x.Value).ToArray());

                    if (propertyName != null) {
                        item.TryCachePropertyValue(propertyName, new PropertyItem(value));
                    } else {
                        item.Value = value;
                    }

                    yield return item;
                }
            }

        }

        public override string ToString() {
            return $"{GetType().Name}[regex='{regex.ToString()}', template={template}, propertyName={propertyName}]";
        }

    }
}
