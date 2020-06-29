namespace Maxstupo.Fsu.Core {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Dsl;
    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;
    using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
    using Maxstupo.Fsu.Core.Filtering;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Providers;
    using Maxstupo.Fsu.Core.Utility;

    public class FsuEngine {
        public IConsole Console { get; }

        protected ITokenizer<TokenType> Tokenizer { get; }

        protected ITokenParser<TokenType, IProcessor> Parser { get; }

        public IInterpreter<IProcessor> Interpreter { get; }

        public IProcessorPipeline Pipeline { get; }


        public IPropertyProviderList PropertyProviders { get; }

        public IPropertyStore PropertyStore { get; }


        public FsuEngine(IConsole console) {
            Console = console;

            Tokenizer = new Tokenizer<TokenType>(TokenType.Invalid, TokenType.Eol, TokenType.Eof);
            Parser = new TokenParser<TokenType, IProcessor>(Console, TokenType.Comment, TokenType.Eol, TokenType.Eof, TokenType.Invalid);

            Interpreter = new Interpreter<TokenType, IProcessor>(Tokenizer, Parser);

            PropertyProviders = new PropertyProviderList(new BasicFilePropertyProvider());
            PropertyStore = new PropertyStore();

            InitTokenizer();
            InitParser();

            Pipeline = new ProcessorPipeline(Console, PropertyProviders, PropertyStore, Interpreter);
        }

        protected virtual void InitTokenizer() {


        }

        protected virtual void InitParser() {
            List<Grammer<TokenType, IProcessor>> grammers = new List<Grammer<TokenType, IProcessor>> {

                new Grammer<TokenType, IProcessor>(TokenType.Pipe) {
                    Rules = { new LookaheadRule<TokenType>(TokenType.Function, TokenType.TextValue, TokenType.StringValue) }
                },

                new Grammer<TokenType, IProcessor>(TokenType.StringValue, TokenType.TextValue) {
                    IncludeTriggerToken = true,
                    Construct = x => new ItemsProcessor(x.Cast<string>()),
                    Rules = {
                        new RepeatingSequenceRule<TokenType>(false,TokenType.Seperator) {
                           new Rule<TokenType>(TokenType.StringValue,TokenType.TextValue)
                        }
                    }
                },

            };

            // TODO: Concat all plugin grammers with the grammers list above.

            // Ensure that all functions have a pipe or be the end of line after them.
            foreach (Grammer<TokenType, IProcessor> grammer in grammers.Where(x => x.TriggerTokenTokens[0].Equals(TokenType.Function))) {
                grammer.Rules.Add(new LookaheadRule<TokenType>(TokenType.Pipe, TokenType.Eol));
            }

            Parser.Clear();
            foreach (Grammer<TokenType, IProcessor> grammer in grammers)
                Parser.Add(grammer);
        }

        public IEnumerable<ProcessorItem> Evaluate(string input) {
            return Evaluate(input.Split('\n'));
        }

        public IEnumerable<ProcessorItem> Evaluate(IEnumerable<string> input) {
            List<Token<TokenType>> tokens = Tokenizer.Tokenize(input).ToList();

#if DEBUG
            Console.WriteLine("\n----------------------- Tokens ----------------------------\n");


            foreach (Token<TokenType> token in tokens) {
                token.WriteLine(Console, 'a');
            }

            Console.WriteLine("\n----------------------- Parsing ----------------------------\n");
#endif

            List<IProcessor> objs = Parser.Parse(tokens);

#if DEBUG
            Console.WriteLine("\n-------------------- Processor List ----------------------\n");


            foreach (IProcessor a in objs) {
                if (a != null)
                    Console.WriteLine(a.ToString());
            }

            Console.WriteLine("\n--------------------- Pipeline Output --------------------\n");
#endif

            return Pipeline.Process(objs);
        }

    }

}
