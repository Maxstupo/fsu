using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Format;
using Maxstupo.Fsu.Core.Lex;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("transform")]
    public class TransformProcessor : IProcessor {

        private readonly FormatTemplate template;

        public TransformProcessor(FormatTemplate template) {
            this.template = template ?? throw new ArgumentNullException(nameof(template));
        }

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            foreach (ProcessorItem item in items)
                item.Value = template.Make(propertyProvider, propertyStore, item);

            return true;
        }

        public static object Construct(string name, TokenList list) {
            if (list.Count != 1)
                return $"has unexpected parameters {(list.HasNext ? list.Next().Location.ToString() : string.Empty)}";

            Token token = list.Next();

            if (token.Type != TokenType.Text && token.Type != TokenType.QuotedText)
                return $"has invalid parameters {token.Location}";

            FormatTemplate template = FormatTemplate.Build(token.Value.Replace('/', '\\'));
            return new TransformProcessor(template);
        }

    }

}
