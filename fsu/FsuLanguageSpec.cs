using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Dsl.Parser;
using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
using Maxstupo.Fsu.Core.Filtering;
using Maxstupo.Fsu.Core.Format;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Processors;
using Maxstupo.Fsu.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu {
    public class FsuLanguageSpec {

        public static void Init(ITokenizer<TokenType> tokenizer, ITokenParser<TokenType, IProcessor> parser, IPropertyProviderList propertyProviderList) {
            InitTokenizer(tokenizer);
            InitParser(parser);

            propertyProviderList.Add(new ExtendedFilePropertyProvider());
        }

        private static void InitTokenizer(ITokenizer<TokenType> tknr) {
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Constant, "files|dirs|directories|top"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Constant, "join|seq|append|replace"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Constant, "nowindow|no-window"));

            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "scan"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "transform"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "glob"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "print"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "filter"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "in"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "out"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "eval"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "exec"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "copy"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "sort"));
            tknr.Add(new TokenDefinition<TokenType>(TokenType.Function, "regex"));
        }

        private static void InitParser(ITokenParser<TokenType, IProcessor> parser) {

            List<Grammer<TokenType, IProcessor>> grammers = new List<Grammer<TokenType, IProcessor>> {

                new Grammer<TokenType, IProcessor>(TokenType.Pipe) {
                    Rules = { new LookaheadRule<TokenType>(TokenType.Function, TokenType.TextValue, TokenType.StringValue) }
                },

                new Grammer<TokenType, IProcessor>(TokenType.StringValue, TokenType.TextValue) {
                    IncludeTriggerToken=true,
                    Construct = x => new ItemsProcessor(x.Cast<string>()),
                    Rules = {
                        new RepeatingSequenceRule<TokenType>(false,TokenType.Seperator) {
                           new Rule<TokenType>(TokenType.StringValue,TokenType.TextValue)
                        }
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "regex") {
                     Construct = x=> new RegexProcessor(x.Get<string>(0), x.Get<string>(1),x.Get<string>(2)),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue,TokenType.StringValue),
                        new OptionalRule<TokenType>("{0}",TokenType.TextValue, TokenType.StringValue),
                        new OptionalRule<TokenType>(null, TokenType.TextValue,TokenType.StringValue)
                    }
                },

                             new Grammer<TokenType, IProcessor>(TokenType.Function, "print") {
                    Construct = x => new PrintProcessor(x.Get<bool>(0), '|'),
                    Rules = {
                    new OptionalRule<TokenType>(false, TokenType.TextValue)
                     }
                 },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "sort") {
                    Construct = x => new SortProcessor(x.Get<string>(0)),
                    Rules = {
                        new OptionalRule<TokenType>("size", TokenType.TextValue, TokenType.StringValue)
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "eval") {
                    Construct = x => new EvalProcessor(x.Get<bool>(0), x.Get<bool>(1)),
                    Rules = {
                        new OptionalRule<TokenType>(TokenType.Constant, false, "join|seq") {
                            TokenConverter = token => token.Value.Equals("join", StringComparison.InvariantCultureIgnoreCase)
                        },
                        new OptionalRule<TokenType>(TokenType.Constant, false, "append|replace") {
                            TokenConverter = token => token.Value.Equals("append", StringComparison.InvariantCultureIgnoreCase)
                        },
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "scan") {
                    Construct = x => new ScanProcessor(x.Get<bool>(0), x.Get<SearchOption>(1)),
                    Rules = {
                        new Rule<TokenType>(TokenType.Constant, "files|dirs|directories") {
                            TokenConverter = token => token.Value.Equals("files", StringComparison.InvariantCultureIgnoreCase)
                        },
                        new OptionalRule<TokenType>(TokenType.Constant, SearchOption.AllDirectories, "top") {
                            TokenConverter = token => token.Value.Equals("top", StringComparison.InvariantCultureIgnoreCase) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories
                        },
                    }
                },


                new Grammer<TokenType, IProcessor>(TokenType.Function, "glob") {
                    Construct = x => new GlobProcessor(x.Get<string>(0)),
                    Rules = { new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "in") {
                    Construct = x => new InProcessor(x.Get<string>(0)),
                    Rules = { new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "out") {
                    Construct = x => new OutProcessor(x.Get<string>(0)),
                    Rules = { new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "transform") {
                    Construct = x => new TransformProcessor(x.Get<FormatTemplate>(0)),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue, TokenType.ItemProperty) {
                            TokenConverter = token => FormatTemplate.Build(token.Value)
                        }
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "exec") {
                    Construct = x => new ExecProcessor(x.Get<FormatTemplate>(0), x.Get<FormatTemplate>(1),x.Get<bool>(2)),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) {
                            TokenConverter = token => FormatTemplate.Build(token.Value)
                        },
                        new OptionalRule<TokenType>(FormatTemplate.Build("@{filepath}"), TokenType.TextValue, TokenType.StringValue) {
                            TokenConverter = token => FormatTemplate.Build(token.Value)
                        },
                        new OptionalRule<TokenType>(TokenType.Constant, false, "nowindow|no-window") {
                            TokenConverter = token => token.Value.Equals("nowindow", StringComparison.InvariantCultureIgnoreCase) || token.Value.Equals("no-window", StringComparison.InvariantCultureIgnoreCase)
                        }
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "copy") {
                    Construct = x=>new CopyProcessor(x.Get<FormatTemplate>(0), x.Get<FormatTemplate>(1)),
                    Rules = {
                         new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) {
                            TokenConverter = token => FormatTemplate.Build(token.Value)
                        },  
                        new OptionalRule<TokenType>(FormatTemplate.Build("@{filepath}"), TokenType.TextValue, TokenType.StringValue) {
                            TokenConverter = token => FormatTemplate.Build(token.Value)
                        },
                    }
                }
            };

            // Filter processor.
            RepeatingSequenceRule<TokenType> rsr = new RepeatingSequenceRule<TokenType>(true, TokenType.LogicOperator) {
                new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue) { TokenConverter = value => value },
                new OptionalRule<TokenType>(TokenType.Unit) { TokenConverter = value => value },
                new OptionalRule<TokenType>(TokenType.Not) { TokenConverter = value => value },
                new Rule<TokenType>(TokenType.StringOperator, TokenType.NumericOperator) { TokenConverter = value => value },
                new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue) { TokenConverter = value => value },
                new OptionalRule<TokenType>(TokenType.Unit) { TokenConverter = value => value },
            };
            rsr.TokenConverter = value => value;

            grammers.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "filter") {
                Construct = x => {
                    Filter.FilterBuilder builder = Filter.Builder();

                    string leftValue = null;
                    string op = null;
                    string rightValue = null;
                    bool invert = false;
                    bool requiresLogic = false;

                    foreach (Token<TokenType> token in x) {

                        switch (token.TokenType) {
                            case TokenType.GlobalProperty:
                            case TokenType.ItemProperty:
                            case TokenType.NumberValue:
                            case TokenType.StringValue:
                            case TokenType.TextValue:
                                if (requiresLogic)
                                    throw new Exception(token.Location);

                                if (leftValue == null) {
                                    leftValue = Regex.Replace(token.Value, @"[\{\}]", string.Empty, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                } else if (rightValue == null) {
                                    if (op == null)
                                        throw new Exception(token.Location);

                                    rightValue = Regex.Replace(token.Value, @"[\{\}]", string.Empty, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                    builder.Condition(leftValue, (invert ? "!" : string.Empty) + op, rightValue);
                                    op = null;
                                    leftValue = null;
                                    rightValue = null;
                                    requiresLogic = true;
                                    invert = false;
                                }
                                break;
                            case TokenType.Not:
                                invert = true;
                                break;
                            case TokenType.LogicOperator:
                                if (token.Value == "&") {
                                    builder.And();
                                    requiresLogic = false;
                                } else if (token.Value == "|") {
                                    builder.Or();
                                    requiresLogic = false;
                                }
                                break;
                            case TokenType.NumericOperator:
                            case TokenType.StringOperator:
                                op = token.Value;
                                break;
                            default:
                                throw new Exception(token.Location);
                        }

                    }

                    return new FilterProcessor(builder.Create());
                },
                Rules = { rsr }
            });

            // Ensure that all functions have a pipe or be the end of line after them.
            foreach (Grammer<TokenType, IProcessor> grammer in grammers.Where(x => x.TriggerTokenTokens[0].Equals(TokenType.Function))) {
                grammer.Rules.Add(new LookaheadRule<TokenType>(TokenType.Pipe, TokenType.Eol));
            }

            foreach (Grammer<TokenType, IProcessor> grammer in grammers)
                parser.Add(grammer);

        }



    }
}
