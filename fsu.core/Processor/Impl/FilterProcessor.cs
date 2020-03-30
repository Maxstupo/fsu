using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Filtering;
using Maxstupo.Fsu.Core.Lex;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Core.Processor.Impl {

    [Function("filter")]
    public class FilterProcessor : IProcessor {

        private readonly Filter filter;

        public FilterProcessor(Filter filter) {
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public bool Process(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ref List<ProcessorItem> items) {
            items.RemoveAll(item => !filter.Check(propertyProvider, propertyStore, item));
            return true;
        }

        public static object Construct(string name, TokenList list) {
            if (list.Count == 0)
                return $"has unexpected parameters";

            Filter.FilterBuilder builder = Filter.Builder();

            string leftval = null;
            string op = null;
            string rightval = null;
            bool requiresLogic = false;
            try {
                while (list.HasNext) {
                    Token token = list.Next();

                    switch (token.Type) {
                        case TokenType.QuotedText:
                        case TokenType.Text:
                        case TokenType.Number:
                            if (requiresLogic)
                                return $"has invalid filter syntax {token.Location}";

                            if (leftval == null) {
                                leftval = token.Value;
                            } else if (rightval == null) {
                                if (op == null)
                                    return $"has invalid filter syntax {token.Location}";

                                rightval = token.Value;
                                builder.Condition(leftval, op, rightval);
                                op = null;
                                leftval = null;
                                rightval = null;
                                requiresLogic = true;
                            }
                            break;

                        case TokenType.Operator:
                            if (token.Value == "&") {
                                builder.And();
                                requiresLogic = false;
                            } else if (token.Value == "|") {
                                builder.Or();
                                requiresLogic = false;
                            } else if (leftval == null) {
                                return $"has invalid filter syntax {token.Location}";
                            } else {
                                op += token.Value;
                            }
                            if (requiresLogic)
                                return $"has invalid filter syntax {token.Location}";
                            break;

                        default:
                            return $"has unexpected symbols {token.Location}";
                    }
                }

                Filter filter = builder.Create();
                return new FilterProcessor(filter);
            } catch (Exception e) {
                return $"has invalid filter syntax {e}";
            }
        }
    }

}
