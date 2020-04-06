﻿using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Dsl.Parser;
using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
using Maxstupo.Fsu.Core.Format;
using Maxstupo.Fsu.Core.Plugins;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Standard.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Maxstupo.Fsu.Standard {
    public class MyFirstPlugin : IFsuPlugin {

        public string PluginId => "fsu.standard";

        public string PluginName => "fsu.standard";

        public string PluginAuthor => "Maxstupo";


        public void Init(IFsuHost host) {
            InitTokenizer(host.Tokenizer);
            InitParser(host.Parser);
        }

        private void InitTokenizer(ITokenizer<TokenType> tokenizer) {
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Constant, "files|dirs|directories|top"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Constant, "join|seq|append|replace"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "scan"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "transform"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "glob"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "print"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "filter"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "in"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "out"));
            tokenizer.Add(new TokenDefinition<TokenType>(TokenType.Function, "eval"));
        }

        private void InitParser(ITokenParser<TokenType, IProcessor> parser) {
            //   parser.Clear();

            List<Grammer<TokenType, IProcessor>> grammers = new List<Grammer<TokenType, IProcessor>> {

                new Grammer<TokenType, IProcessor>(TokenType.Pipe) {
                    Rules = {
                        new LookaheadRule<TokenType>(TokenType.Function,TokenType.TextValue,TokenType.StringValue)
                    }
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

                new Grammer<TokenType, IProcessor>(TokenType.Function, "print") {
                    Construct = x => new PrintProcessor()
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "eval") {
                    Construct = x => new EvalProcessor(x.Get<bool>(0), x.Get<bool>(1)),
                    Rules = {
                        new OptionalRule<TokenType>(TokenType.Constant, false, "join|seq") {
                            ValueConverter = value => value.Equals("join", StringComparison.InvariantCultureIgnoreCase)
                        },
                        new OptionalRule<TokenType>(TokenType.Constant, true, "append|replace") {
                            ValueConverter = value => value.Equals("append", StringComparison.InvariantCultureIgnoreCase)
                        },
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "scan") {
                    Construct = x => new ScanProcessor(x.Get<bool>(0), x.Get<SearchOption>(1)),
                    Rules = {
                        new Rule<TokenType>(TokenType.Constant, "files|dirs|directories") {
                            ValueConverter = value => value.Equals("files", StringComparison.InvariantCultureIgnoreCase)
                        },
                        new OptionalRule<TokenType>(TokenType.Constant, SearchOption.AllDirectories, "top") {
                            ValueConverter = value => value.Equals("top", StringComparison.InvariantCultureIgnoreCase) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories
                        },
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "filter") {
                    Construct = x => new FilterProcessor(),
                    Rules = {
                        new RepeatingSequenceRule<TokenType>(true, TokenType.LogicOperator) {
                            new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue),
                            new OptionalRule<TokenType>(TokenType.Unit),
                            new OptionalRule<TokenType>(TokenType.Not),
                            new Rule<TokenType>(TokenType.StringOperator, TokenType.NumericOperator),
                            new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue),
                            new OptionalRule<TokenType>(TokenType.Unit),
                        }
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "glob") {
                    Construct = x => new GlobProcessor(x.Get<string>(0)),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "in") {
                    Construct = x => new InProcessor(x.Get<string>(0)),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "out") {
                    Construct = x => new OutProcessor(x.Get<string>(0)),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                    }
                },

                new Grammer<TokenType, IProcessor>(TokenType.Function, "transform") {
                    Construct = x => new TransformProcessor(x.Get<FormatTemplate>(0)),
                    Rules = {
                        new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue) {
                            ValueConverter = value => FormatTemplate.Build(value)
                        },
                    }
                }
            };

            // Ensure that all functions have a pipe or be the end of line after them.
            foreach (Grammer<TokenType, IProcessor> grammer in grammers.Where(x => x.TriggerTokenTokens.Equals(TokenType.Function))) {
                grammer.Rules.Add(new LookaheadRule<TokenType>(TokenType.Pipe, TokenType.Eol));
            }

            foreach (Grammer<TokenType, IProcessor> grammer in grammers)
                parser.Add(grammer);

        }


        public void Enable() { }

        public void Disable() { }

    }
}
