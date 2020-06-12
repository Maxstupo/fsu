using GlobExpressions;
using Maxstupo.Fsu.Core.Processor;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Processors {
    public class GlobProcessor : IProcessor {

        private readonly Glob glob;

        public GlobProcessor(string pattern, bool matchFullPath = false, bool caseSensitive = false) {
            GlobOptions options = GlobOptions.Compiled;

            if (matchFullPath)
                options |= GlobOptions.MatchFullPath;

            if (!caseSensitive)
                options |= GlobOptions.CaseInsensitive;

            glob = new Glob(pattern, options);
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            return items.Where(x => glob.IsMatch(x.Value));
        }

        public override string ToString() {
            return $"{GetType().Name}[pattern='{glob.Pattern}']";
        }
    }
}
