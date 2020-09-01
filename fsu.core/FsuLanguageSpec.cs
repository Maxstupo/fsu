namespace Maxstupo.Fsu.Core {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
                new TokenDefinition<TokenType>(TokenType.Constant, "files|dirs|directories|top|all"),
                new TokenDefinition<TokenType>(TokenType.Constant, "join|seq|append|replace"),
                new TokenDefinition<TokenType>(TokenType.Constant, "window|nowindow|no-window"),
                new TokenDefinition<TokenType>(TokenType.Constant, "all|matches"),
                new TokenDefinition<TokenType>(TokenType.Constant, "wait"),
                new TokenDefinition<TokenType>(TokenType.Constant, "asc|desc"),
                new TokenDefinition<TokenType>(TokenType.Keyword, "from|to|as|pass"),

                new TokenDefinition<TokenType>(TokenType.Function, "scan"),
                new TokenDefinition<TokenType>(TokenType.Function, "transform"),
                new TokenDefinition<TokenType>(TokenType.Function, "sum"),
                new TokenDefinition<TokenType>(TokenType.Function, "rename"),
                new TokenDefinition<TokenType>(TokenType.Function, "extract"),
                new TokenDefinition<TokenType>(TokenType.Function, "glob"),
                new TokenDefinition<TokenType>(TokenType.Function, "print"),
                new TokenDefinition<TokenType>(TokenType.Function, "filter"),
                new TokenDefinition<TokenType>(TokenType.Function, "in"),
                new TokenDefinition<TokenType>(TokenType.Function, "out"),
                new TokenDefinition<TokenType>(TokenType.Function, "eval"),
                new TokenDefinition<TokenType>(TokenType.Function, "exec"),
                new TokenDefinition<TokenType>(TokenType.Function, "sort"),
                new TokenDefinition<TokenType>(TokenType.Function, "avg"),
                new TokenDefinition<TokenType>(TokenType.Function, "min"),
                new TokenDefinition<TokenType>(TokenType.Function, "max"),
                new TokenDefinition<TokenType>(TokenType.Function, "mkdir"),
            };
        }

        private static readonly Func<Token<TokenType>, object> TokenConverterFormatTemplate = token => FormatTemplate.Build(token.Value);
        private static readonly Func<Token<TokenType>, object> TokenConverterPropertyName = token => {
            string propertyName = token.Value.Substring(2);
            return propertyName.Substring(0, propertyName.Length - 1);
        };

        public static ISet<Grammar<TokenType, IProcessor>> GetGrammers() {
            // Filter processor.
            RepeatingSequenceRule<TokenType> rsr = new RepeatingSequenceRule<TokenType>(true, TokenType.LogicOperator) {
                new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue) { TokenConverter = value => value },
                new OptionalRule<TokenType>(TokenType.Unit) { TokenConverter = value => value },
                new OptionalRule<TokenType>(TokenType.Ignore) { TokenConverter = value => value },
                new OptionalRule<TokenType>(TokenType.Not) { TokenConverter = value => value },
                new Rule<TokenType>(TokenType.Operator) { TokenConverter = value => value },
                new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue) { TokenConverter = value => value },
                new OptionalRule<TokenType>(TokenType.Unit) { TokenConverter = value => value },
            };
            rsr.TokenConverter = value => value;


            return new HashSet<Grammar<TokenType, IProcessor>> {

                new Grammar<TokenType, IProcessor>(TokenType.Function, "print") {
                    Construct = x => new PrintProcessor(x.Get<string>("sep")),
                    Rules = {
                        new OptionalRule<TokenType>(null, TokenType.TextValue, TokenType.StringValue) {Key="sep"}
                    }
                },

                //new Grammar<TokenType, IProcessor>(TokenType.Function, "sort") {
                //    Construct = x => new SortProcessor(x.Get<string>(0), x.Get<bool>(1)),
                //    Rules = {
                //        new OptionalRule<TokenType>("filesize", TokenType.TextValue, TokenType.StringValue),
                //        new OptionalRule<TokenType>(TokenType.Constant, "asc", "asc|desc") {
                //            TokenConverter = token => token.Value.Equals("asc", StringComparison.InvariantCultureIgnoreCase)
                //        }
                //    }
                //},

                //new Grammar<TokenType, IProcessor>(TokenType.Function, "eval") {
                //    Construct = x => new EvalProcessor(x.Get<bool>(0), x.Get<bool>(1)),
                //    Rules = {
                //        new OptionalRule<TokenType>(TokenType.Constant, false, "join|seq") {
                //            TokenConverter = token => token.Value.Equals("join", StringComparison.InvariantCultureIgnoreCase)
                //        },
                //        new OptionalRule<TokenType>(TokenType.Constant, false, "append|replace") {
                //            TokenConverter = token => token.Value.Equals("append", StringComparison.InvariantCultureIgnoreCase)
                //        },
                //    }
                //},

                new Grammar<TokenType, IProcessor>(TokenType.Function, "scan") {
                    Construct = x => new ScanProcessor(x.Get<bool>("isFiles"), x.Get<SearchOption>("mode")),
                    Rules = {
                        new OptionalRule<TokenType>(TokenType.Constant, SearchOption.AllDirectories, "top|all") {
                            Key="mode",
                            TokenConverter = token => token.Value.Equals("top", StringComparison.InvariantCultureIgnoreCase) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories
                        },
                        new Rule<TokenType>(TokenType.Constant, "files|dirs|directories") {
                            Key="isFiles",
                            TokenConverter = token => token.Value.Equals("files", StringComparison.InvariantCultureIgnoreCase)
                        },

                    }
                },


                new Grammar<TokenType, IProcessor>(TokenType.Function, "glob") {
                    Construct = x => new GlobProcessor(x.Get<string>("pattern")),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) { Key = "pattern" }
                    }
                },

                new Grammar<TokenType, IProcessor>(TokenType.Function, "in") {
                    Construct = x => new InProcessor(x.Get<string>("filepath")),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) { Key = "filepath" }
                    }
                },

                new Grammar<TokenType, IProcessor>(TokenType.Function, "out") {
                    Construct = x => new OutProcessor(x.Get<string>("filepath")),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) { Key = "filepath" }
                    }
                },

                new Grammar<TokenType, IProcessor>(TokenType.Function, "transform") {
                    Construct = x => new TransformProcessor(x.Get<FormatTemplate>("template")),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue, TokenType.ItemProperty) {
                            Key = "template",
                            TokenConverter = TokenConverterFormatTemplate
                        }
                    }
                },


                new Grammar<TokenType, IProcessor>(TokenType.Function, "extract") {
                    Construct = x => {
                 Console.WriteLine(string.Join("\n",x));
                        //if (x.Get<int>(1) == 0) { // first branch
                        //    return new ExtractProcessor(x.Get<string>(0), x.Get<string>(5), x.Get<string>(3), x.Get<bool>(7));

                        //} else { // second branch
                        //    return new ExtractProcessor(x.Get<string>(0), x.Get<string>(3), "value", x.Get<bool>(5));
                        //}
                        return null;
                    },
                    Rules = {
                       new Rule<TokenType>(TokenType.StringValue),
                       new BranchingRule<TokenType>() {
                           Branches = {
                                //extract "regex" from @{name} as @{ep} 
                                new List<Rule<TokenType>>() {
                                    new Rule<TokenType>(TokenType.Keyword, "from"),
                                    new Rule<TokenType>(TokenType.ItemProperty) { Key="from", TokenConverter = TokenConverterPropertyName },
                                    new Rule<TokenType>(TokenType.Keyword, "as"),
                                    new Rule<TokenType>(TokenType.ItemProperty) { Key="as", TokenConverter = TokenConverterPropertyName },

                                },
                                // extract "regex" as @{ep}
                                new List<Rule<TokenType>>() {
                                    new Rule<TokenType>(TokenType.Keyword, "as"),
                                    new Rule<TokenType>(TokenType.ItemProperty) { Key = "as",TokenConverter = TokenConverterPropertyName },
                                }
                           }
                       },
                       new OptionalGroupRule<TokenType>("pass",true) {
                            Rules = {
                                new Rule<TokenType>(TokenType.Keyword, "pass"),
                                new Rule<TokenType>(TokenType.Constant,  "all|matches") {
                                    TokenConverter = token => token.Value.Equals("all", StringComparison.InvariantCultureIgnoreCase)
                                }
                            }
                       }
                    }
                },


                new Grammar<TokenType, IProcessor>(TokenType.Function, "rename") {
                    Construct = x => new RenameProcessor(x.Get<FormatTemplate>("path")),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue, TokenType.ItemProperty, TokenType.GlobalProperty) {
                            Key = "path",
                            TokenConverter = TokenConverterFormatTemplate
                        }
                    }
                },

                //new Grammar<TokenType, IProcessor>(TokenType.Function, "exec") {
                //    Construct = x => new ExecProcessor(x.Get<FormatTemplate>(0), x.Get<FormatTemplate>(1), x.Get<bool>(2),x.Get<bool>(3)),
                //    Rules = {
                //        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) {
                //            TokenConverter = TokenConverterFormatTemplate
                //        },
                //        new OptionalRule<TokenType>(FormatTemplate.Build("@{filepath}"), TokenType.TextValue, TokenType.StringValue) {
                //            TokenConverter = TokenConverterFormatTemplate
                //        },
                //        new OptionalRule<TokenType>(TokenType.Constant, false, "window|nowindow|no-window") {
                //            TokenConverter = token => token.Value.Equals("nowindow", StringComparison.InvariantCultureIgnoreCase) || token.Value.Equals("no-window", StringComparison.InvariantCultureIgnoreCase)
                //        },
                //        new OptionalRule<TokenType>(TokenType.Constant, false, "wait") {
                //            TokenConverter = token => token.Value.Equals("wait", StringComparison.InvariantCultureIgnoreCase)
                //        }
                //    }
                //},

                //new Grammar<TokenType, IProcessor>(TokenType.Function, "filter") {
                //    Construct = x => {
                //        Filter.FilterBuilder builder = Filter.Builder();

                //        Token<TokenType> leftValueToken=null;
                //        string op = null;
                //        bool ignore = false;
                //        bool invert = false;
                //        bool requiresLogic = false;

                //        foreach (Token<TokenType> token in x) {

                //            switch (token.TokenType) {
                //                case TokenType.GlobalProperty:
                //                case TokenType.ItemProperty:
                //                case TokenType.NumberValue:
                //                case TokenType.StringValue:
                //                case TokenType.TextValue:
                //                    if (requiresLogic)
                //                        throw new Exception(token.Location);

                //                    if (leftValueToken == null) {
                //                        leftValueToken = token;
                //                    } else  {
                //                        if (op == null)
                //                            throw new Exception(token.Location);

                //                        string leftValue = GetOperandValue(leftValueToken);
                //                        string rightValue = GetOperandValue(token);

                //                        Operand opLeft = GetOperand(leftValueToken);
                //                        Operand opRight = GetOperand(token);

                //                        Operator oper = GetOperator(op,ignore, invert);

                //                        builder.Condition(opLeft, oper, opRight);

                //                        op = null;
                //                        leftValueToken = null;
                //                        requiresLogic = true;
                //                        invert = false;
                //                    }
                //                    break;
                //                case TokenType.Ignore:
                //                    ignore = true;
                //                    break;
                //                case TokenType.Not:
                //                    invert = true;
                //                    break;
                //                case TokenType.LogicOperator:
                //                    if (token.Value == "&") {
                //                        builder.And();
                //                        requiresLogic = false;
                //                    } else if (token.Value == "|") {
                //                        builder.Or();
                //                        requiresLogic = false;
                //                    }
                //                    break;
                //                case TokenType.Operator:
                //                    op = token.Value;
                //                    break;
                //                default:
                //                    throw new Exception(token.Location);
                //            }

                //        }
                //        return new FilterProcessor(builder.Create());
                //    },
                //    Rules = { rsr }
                //},

                //new Grammar<TokenType, IProcessor>(TokenType.Function, "avg") {
                //    Construct = x => new AvgProcessor(x.Get<string>(0), x.Get<string>(1)),
                //    Rules = {
                //        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                //        new OptionalRule<TokenType>(null, TokenType.TextValue, TokenType.StringValue)
                //    }
                //},
                //  new Grammar<TokenType, IProcessor>(TokenType.Function, "sum") {
                //    Construct = x => new SumProcessor(x.Get<string>(0), x.Get<string>(1)),
                //    Rules = {
                //        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                //        new OptionalRule<TokenType>(null, TokenType.TextValue, TokenType.StringValue)
                //    }
                //},
                //new Grammar<TokenType, IProcessor>(TokenType.Function, "min") {
                //    Construct = x => new MinProcessor(x.Get<string>(0), x.Get<string>(1)),
                //    Rules = {
                //        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                //        new OptionalRule<TokenType>(null, TokenType.TextValue, TokenType.StringValue)
                //    }
                //},
                //new Grammar<TokenType, IProcessor>(TokenType.Function, "max") {
                //    Construct = x => new MaxProcessor(x.Get<string>(0), x.Get<string>(1)),
                //    Rules = {
                //        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                //        new OptionalRule<TokenType>(null, TokenType.TextValue, TokenType.StringValue)
                //    }
                //},
                //new Grammar<TokenType, IProcessor>(TokenType.Function, "mkdir") {
                //    Construct = x => new MkDirProcessor(x.Get<FormatTemplate>(0)),
                //    Rules = {
                //        new OptionalRule<TokenType>(FormatTemplate.Build("@{value}"), TokenType.TextValue, TokenType.StringValue) {
                //            TokenConverter = TokenConverterFormatTemplate
                //        },
                //    }
                //}

            };
        }

        private static OperandType GetOperandType(Token<TokenType> token) {
            switch (token.TokenType) {
                case TokenType.GlobalProperty: return OperandType.GlobalProperty;
                case TokenType.ItemProperty: return OperandType.ItemProperty;
                case TokenType.NumberValue: return OperandType.NumericConstant;
                default: return OperandType.TextConstant;
            }
        }

        private static string GetOperandValue(Token<TokenType> token) {
            if (token.TokenType == TokenType.GlobalProperty || token.TokenType == TokenType.ItemProperty) {
                return Regex.Replace(token.Value, @"[\@\$\{\}]", string.Empty, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            } else {
                return token.Value;
            }
        }

        private static Operand GetOperand(Token<TokenType> token) {
            string value = GetOperandValue(token);

            OperandType type = GetOperandType(token);

            if (type == OperandType.NumericConstant) {
                return new Operand(double.Parse(value));
            } else {
                return new Operand(value, type, null);
            }
        }

        private static Operator GetOperator(string op, bool ignored, bool inverted) {
            Operator oper = 0;

            char c = op[0];
            bool hasSecond = op.Length == 2;

            if (c == '<') {

                if (hasSecond && op[1] == '>') { // <>
                    oper |= Operator.Regex;

                } else { // <
                    oper |= Operator.LessThan;

                }

            } else if (c == '>') {
                if (hasSecond && op[1] == '<') { // ><
                    oper |= Operator.Contains;

                } else if (hasSecond && op[1] == '~') { // >~
                    oper |= Operator.StartsWith;

                } else { // >
                    oper |= Operator.GreaterThan;

                }
            } else if (c == '~' && hasSecond && op[1] == '<') { // ~<
                oper |= Operator.EndsWith;
            }


            if (c == '=' || (hasSecond && op[1] == '=')) // <=, >=, =
                oper |= Operator.Equal;



            if (inverted) oper |= Operator.Not;
            if (ignored) oper |= Operator.Ignore;

            return oper;
        }

    }

}
