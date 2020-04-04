using Maxstupo.Fsu;
using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Dsl.Parser;
using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
using Maxstupo.Fsu.Core.Plugins;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Processor.Standard;
using System.IO;

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

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Pipe));

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "print") {
                Construct = x => new PrintProcessor()
            });
            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "eval"));

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "scan") {
                Construct = x => new ScanProcessor(true, SearchOption.AllDirectories),
                Rules = {
                    new Rule<TokenType>(TokenType.Constant, "files|dirs|directories"),
                    new OptionalRule<TokenType>(TokenType.Constant, "top"),
                }
            });

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "filter") {
                Construct = x => new FilterProcessor(),
                Rules = {
                    new RepeatingSequenceRule<TokenType>(TokenType.LogicOperator) {
                        new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue),
                        new OptionalRule<TokenType>(TokenType.Not),
                        new Rule<TokenType>(TokenType.StringOperator, TokenType.NumericOperator),
                        new Rule<TokenType>(TokenType.ItemProperty, TokenType.GlobalProperty, TokenType.NumberValue, TokenType.StringValue, TokenType.TextValue),
                    }
                }
            });

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "glob") {
                Construct = x => new GlobProcessor("*"),
                Rules = {
                     new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                }
            });

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "in") {
                Construct = x => new InProcessor(""),
                Rules = {
                    new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                }
            });

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "out") {
                Construct = x => new OutProcessor(""),
                Rules = {
                    new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                }
            });

            parser.Add(new Grammer<TokenType, IProcessor>(TokenType.Function, "transform") {
                //Construct = x => new InProcessor(""),
                Rules = {
                    new Rule<TokenType>(TokenType.TextValue, TokenType.StringValue),
                }
            });
        }


        public void Enable() { }

        public void Disable() { }

    }
}
