namespace Maxstupo.Fsu {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Dsl;
    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using Maxstupo.Fsu.Providers;
    using Maxstupo.Fsu.Utility;

    public class Program {

        private string Title => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

        public IConsole Console { get; }

        public ITokenizer<TokenType> Tokenizer { get; }

        public ITokenParser<TokenType, IProcessor> Parser { get; }

        public IInterpreter<IProcessor> Interpreter { get; }

        public IProcessorPipeline Pipeline { get; }


        public Cli Cli { get; }


        public Program() {
            // System.Console.OutputEncoding = Encoding.Unicode;
            System.Console.Title = Title;

            Console = new ColorConsole(System.Console.Out);

            Tokenizer = new Tokenizer<TokenType>(TokenType.Invalid, TokenType.Eol, TokenType.Eof);
            Parser = new TokenParser<TokenType, IProcessor>(Console, TokenType.Comment, TokenType.Eol, TokenType.Eof, TokenType.Invalid);

            Interpreter = new Interpreter<TokenType, IProcessor>(Tokenizer, Parser);

            IPropertyProviderList propertyProviders = new PropertyProviderList(new BasicFilePropertyProvider());
            IPropertyStore propertyStore = new PropertyStore();
            Pipeline = new ProcessorPipeline(Console, propertyProviders, propertyStore, Interpreter);

            FsuLanguageSpec.Init(Tokenizer, Parser, propertyProviders);

            Cli = new Cli(Console);
            Cli.OnCommand += Cli_OnCommand;

            //Temp
            //   if (File.Exists("fsu_on_start.txt"))
            //        Cli_OnCommand(null, File.ReadAllText("fsu_on_start.txt"));
        }

        public void Run() {
            Cli.Run();
        }



        private void Cli_OnCommand(object sender, string input) {
            //     Console.Clear();
            List<Token<TokenType>> tokens = Tokenizer.Tokenize(input.Split('\n')).ToList();

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

            Pipeline.Process(objs);

        }




        [STAThread]
        static int Main(string[] args) {
            Program program = new Program();
            program.Run();


            return 0;
        }

    }

}
