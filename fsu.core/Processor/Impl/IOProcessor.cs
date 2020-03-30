using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Lex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("out")]
    public class OutProcessor : IProcessor {

        private readonly string filepath;

        public OutProcessor(string filepath) {
            this.filepath = filepath ?? throw new ArgumentNullException(nameof(filepath));
        }

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            File.WriteAllLines(filepath, items.Select(item => item.Value), Encoding.UTF8);
            return true;
        }

        public static object Construct(string name, TokenList list) {
            if (list.Count != 1)
                return $"has unexpected parameters {list.Next().Location}";

            Token token = list.Next();

            if (token.Type != TokenType.Text && token.Type != TokenType.QuotedText)
                return $"has invalid parameters {token.Location}";

            return new OutProcessor(token.Value);
        }

    }

    [Function("in")]
    public class InProcessor : IProcessor {

        private readonly string filepath;

        public InProcessor(string filepath) {
            this.filepath = filepath ?? throw new ArgumentNullException(nameof(filepath));
        }

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            items.AddRange(File.ReadAllLines(filepath, Encoding.UTF8).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => new ProcessorItem(line)));
            return true;
        }

        public static object Construct(string name, TokenList list) {
            if (list.Count != 1)
                return $"has unexpected parameters {list.Next().Location}";

            Token token = list.Next();

            if (token.Type != TokenType.Text && token.Type != TokenType.QuotedText)
                return $"has invalid parameters {token.Location}";

            if (!File.Exists(token.Value))
                return $"the specified filepath doesn't exist {token.Location}";

            return new InProcessor(token.Value);
        }

    }

}
