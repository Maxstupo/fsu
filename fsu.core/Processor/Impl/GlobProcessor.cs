using GlobExpressions;
using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("glob")]
    public class GlobProcessor : IProcessor {

        private readonly Glob glob;

        public GlobProcessor(string pattern, bool matchFullPath) {
            GlobOptions options = GlobOptions.Compiled | GlobOptions.CaseInsensitive;
            if (matchFullPath)
                options |= GlobOptions.MatchFullPath;

            glob = new Glob(pattern, options);
        }

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            items.RemoveAll(item => !glob.IsMatch(item.Value));
            return true;
        }

        public static object Construct(string name, TokenList list) {

            if (list.Count == 0 || list.Count > 2)
                return $"has unexpected parameters {list.Next().Location}";

            Token token1 = list.Next();
            Token token2 = list.HasNext ? list.Next() : null;

            if (token1.Type != TokenType.QuotedText && token1.Type != TokenType.Text)
                return $"has invalid parameters {token1.Location}";


            string pattern = token1.Value;
            bool matchFullPath = false;

            if (token2 != null) {
                if (!token1.Value.Equals("path", StringComparison.InvariantCultureIgnoreCase))
                    return $"has invalid parameters {token1.Location}";

                matchFullPath = true;
                pattern = token2.Value;
            }

            return new GlobProcessor(pattern, matchFullPath);

        }

    }

}
