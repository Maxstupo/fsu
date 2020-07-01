namespace Maxstupo.Fsu.Core {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;
    using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
    using Maxstupo.Fsu.Core.Filtering;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Processor.Processors;

    public static class FsuLanguageSpec {

        public static ISet<TokenDefinition<TokenType>> GetTokenDefinitions() {
            return new HashSet<TokenDefinition<TokenType>> {
                new TokenDefinition<TokenType>(TokenType.Constant, "files|dirs|directories|top"),
                new TokenDefinition<TokenType>(TokenType.Constant, "join|seq|append|replace"),
                new TokenDefinition<TokenType>(TokenType.Constant, "nowindow|no-window"),

                new TokenDefinition<TokenType>(TokenType.Function, "scan"),
                new TokenDefinition<TokenType>(TokenType.Function, "transform"),
                new TokenDefinition<TokenType>(TokenType.Function, "glob"),
                new TokenDefinition<TokenType>(TokenType.Function, "print"),
                new TokenDefinition<TokenType>(TokenType.Function, "filter"),
                new TokenDefinition<TokenType>(TokenType.Function, "in"),
                new TokenDefinition<TokenType>(TokenType.Function, "out"),
                new TokenDefinition<TokenType>(TokenType.Function, "eval"),
                new TokenDefinition<TokenType>(TokenType.Function, "exec"),
                new TokenDefinition<TokenType>(TokenType.Function, "copy"),
                new TokenDefinition<TokenType>(TokenType.Function, "sort"),
            };
        }

        public static ISet<Grammer<TokenType, IProcessor>> GetGrammers() {
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


            return new HashSet<Grammer<TokenType, IProcessor>> {

                new Grammer<TokenType, IProcessor>(TokenType.Function, "print") {
                    Construct = x => new PrintProcessor(x.Get<string>(0)),
                    Rules = {
                        new OptionalRule<TokenType>(null, TokenType.TextValue, TokenType.StringValue)
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
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "filter") {
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
                }

            };
        }

    }

}
