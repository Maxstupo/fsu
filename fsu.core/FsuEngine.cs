namespace Maxstupo.Fsu.Core {

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Dsl;
    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;
    using Maxstupo.Fsu.Core.Dsl.Parser.Rules;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Processor.Processors;
    using Maxstupo.Fsu.Core.Providers;
    using Maxstupo.Fsu.Core.Utility;

    public class FsuEngine {

        public IOutput Output { get; }

        public ITokenizer<TokenType> Tokenizer { get; }

        public ITokenParser<TokenType, IProcessor> Parser { get; }

        public IProcessorPipeline Pipeline { get; }

        public IPropertyProviderList PropertyProviders { get; }

        public IPropertyStore PropertyStore { get; }

        public string[] FallbackItems { get; set; }

        private readonly IInterpreter<IProcessor> interpreter;

        public FsuEngine(IOutput output) {
            Output = output;

            Tokenizer = new Tokenizer<TokenType>(TokenType.Invalid, TokenType.Eol, TokenType.Eof, false);
            Tokenizer.OnDefinitionAdded += (sender, definition) => {
                output.WriteLine(Level.Fine, $"Added Token: '&-e;{definition.Regex}&-^;' &-9;{definition.Precedence}&-^; (&-a;{definition.TokenType}&-^;)");
            };

            Parser = new TokenParser<TokenType, IProcessor>(TokenType.Comment, TokenType.Eol, TokenType.Eof, TokenType.Invalid);
            Parser.OnGrammerAdded += (sender, grammer) => {
                output.WriteLine(Level.Fine, $"Added Grammer: '&-b;{grammer.TriggerTokenValuePattern}&-^;' (&-a;{string.Join(", ", grammer.TriggerTokens)}&-^;) with {grammer.Rules.Count} rule(s)");
            };

            Parser.OnTokenError += (sender, token) => {
                output.WriteLine(Level.Error, $"&-c;ERROR - Unexpected token: '{token.Value}' ({token.TokenType}) {token.Location}&-^;");
            };
            Parser.OnTokenParsing += (sender, token) => {
                token.WriteLine(output, Level.Debug);
            };
            interpreter = new Interpreter<TokenType, IProcessor>(Tokenizer, Parser);

            PropertyProviders = new PropertyProviderList(new BasicFilePropertyProvider());
            PropertyStore = new PropertyStore();

            Pipeline = new ProcessorPipeline(Output, PropertyProviders, PropertyStore, interpreter);

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
                        new RepeatingSequenceRule<TokenType>(false, TokenType.Seperator) {
                           new Rule<TokenType>(TokenType.StringValue, TokenType.TextValue)
                        },
                        new LookaheadRule<TokenType>(TokenType.Pipe,TokenType.Eol)
                    }
                },

            };



            Tokenizer.LoadTokenDefinitions();
            foreach (TokenDefinition<TokenType> def in FsuLanguageSpec.GetTokenDefinitions())
                Tokenizer.Add(def);



            IEnumerable<Grammer<TokenType, IProcessor>> allGrammers = grammers.Concat(FsuLanguageSpec.GetGrammers());

            // Ensure that all functions have a pipe or be the end of line after them.
            foreach (Grammer<TokenType, IProcessor> grammer in allGrammers.Where(x => x.TriggerTokens[0].Equals(TokenType.Function)))
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

            Output.WriteLine(Level.Debug, "\n----------------------- Tokens ----------------------------\n");

            foreach (Token<TokenType> token in tokens)
                token.WriteLine(Output, Level.Debug, 'a');

            Output.WriteLine(Level.Debug, "\n----------------------- Parsing ----------------------------\n");


            List<IProcessor> objs = Parser.Parse(tokens);

            // If no initial items are provided, use the fallback.
            if (FallbackItems != null && FallbackItems.Length > 0 && objs.Count > 0 && !(objs[0] is ItemsProcessor) && !(objs[0] is InProcessor))
                objs.Insert(0, new ItemsProcessor(FallbackItems));

            Output.WriteLine(Level.Debug, "\n-------------------- Processor List ----------------------\n");

            foreach (IProcessor a in objs) {
                Output.WriteLine(Level.Debug, a.ToString());
            }

            Output.WriteLine(Level.Debug, "\n--------------------- Pipeline Output --------------------\n");


            return Pipeline.Process(objs);
        }

    }

}
