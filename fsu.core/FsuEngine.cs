namespace Maxstupo.Fsu.Core {

    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Dsl;
    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;
    using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Providers;
    using Maxstupo.Fsu.Core.Utility;

    public class FsuEngine {

        public IConsole Console { get; }

        public ITokenizer<TokenType> Tokenizer { get; }

        public ITokenParser<TokenType, IProcessor> Parser { get; }

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

            Pipeline = new ProcessorPipeline(Console, PropertyProviders, PropertyStore, Interpreter);

            Init();
        }

        protected virtual void Init() {
            ISet<Grammer<TokenType, IProcessor>> grammers = new HashSet<Grammer<TokenType, IProcessor>> {

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



            Tokenizer.Clear();
            Tokenizer.LoadTokenDefinitions();

            foreach (TokenDefinition<TokenType> def in FsuLanguageSpec.GetTokenDefinitions())
                Tokenizer.Add(def);



            IEnumerable<Grammer<TokenType, IProcessor>> allGrammers = grammers.Concat(FsuLanguageSpec.GetGrammers());

            // Ensure that all functions have a pipe or be the end of line after them.
            foreach (Grammer<TokenType, IProcessor> grammer in allGrammers.Where(x => x.TriggerTokenTokens[0].Equals(TokenType.Function)))
                grammer.Rules.Add(new LookaheadRule<TokenType>(TokenType.Pipe, TokenType.Eol));

            Parser.Clear();
            foreach (Grammer<TokenType, IProcessor> grammer in allGrammers)
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
