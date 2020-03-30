using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using Maxstupo.Fsu.Core.Utility;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("print")]
    public class PrintProcessor : IProcessor {

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            for (int i = 0; i < items.Count; i++)
                ColorConsole.WriteLine($"&-8;{i,3}:&-^; {items[i].Value}");

            return true;
        }

        public static object Construct(string name, TokenList list) {
            if (list.Count > 0) 
                return $"has unexpected parameters {list.Next().Location}";
            return new PrintProcessor();
        }

    }

}
