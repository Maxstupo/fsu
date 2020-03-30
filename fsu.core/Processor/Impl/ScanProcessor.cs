using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("scan")]
    public class ScanProcessor : IProcessor {

        private readonly bool findDirectories;
        private readonly SearchOption searchOption;

        public ScanProcessor(bool findDirectories, SearchOption searchOption) {
            this.findDirectories = findDirectories;
            this.searchOption = searchOption;
        }

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            List<ProcessorItem> startingPoints = new List<ProcessorItem>(items);

            items.Clear();

            foreach(ProcessorItem item in startingPoints) {
                if (!Directory.Exists(item.Value))
                    continue;


                IEnumerable<string> discoveredItems = 
                                                 findDirectories ?
                                                    Directory.EnumerateDirectories(item.Value, "*.*", searchOption)
                                                    :
                                                    Directory.EnumerateFiles(item.Value, "*.*", searchOption);

                items.AddRange(discoveredItems.Select(filepath => new ProcessorItem(filepath) { Origin = item.Value }));
            }

            return true;
        }

        public static object Construct(string name, TokenList list) {

            if (list.Count == 0 || list.Count > 2)
                return $"has unexpected parameters {list.Next().Location}";

            Token token = list.Next();


            string[] validNames = { "dir", "dirs", "directories", "file", "files" };

            if (token.Type != TokenType.Text || !validNames.Contains(token.Value, StringComparer.InvariantCultureIgnoreCase))
                return $"has invalid parameters {token.Location}";


            bool findDirectories = new string[] { "dir", "dirs", "directories" }.Contains(token.Value, StringComparer.InvariantCultureIgnoreCase);


            SearchOption searchOption = SearchOption.AllDirectories;

            if (list.HasNext) {

                token = list.Next();

                if (token.Type != TokenType.Text || (!token.Value.Equals("all", StringComparison.InvariantCultureIgnoreCase) && !token.Value.Equals("top", StringComparison.InvariantCultureIgnoreCase)))
                    return $"has invalid parameters {token.Location}";

                searchOption = token.Value.Equals("top", StringComparison.InvariantCultureIgnoreCase) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            }


            return new ScanProcessor(findDirectories, searchOption);
        }

    }

}
