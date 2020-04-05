using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Dsl;
using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Dsl.Parser;
using Maxstupo.Fsu.Core.Plugins;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maxstupo.Fsu {

    public class Program : IFsuHost {

        public IPluginManager PluginManager { get; }

        public IConsole Console { get; }

        public ITokenizer<TokenType> Tokenizer { get; }

        public ITokenParser<TokenType, IProcessor> Parser { get; }

        public IDslInterpreter<IProcessor> Interpreter { get; }

        public IProcessorPipeline Pipeline { get; }

        public IPropertyProviderList PropertyProviders { get; }


        private readonly Cli cli;

        /*
*private readonly IConsole console = new ColorConsole();


private readonly Cli cli;


private readonly IPropertyProvider propertyProvider;
private readonly IPropertyStore propertyStore;

private readonly IProcessorPipeline pipeline;


private readonly FsuTokenizer tokenizer = new FsuTokenizer();
private readonly FsuTokenParser parser;
*/

        public Program() {
            System.Console.OutputEncoding = Encoding.Unicode;
            Console = new ColorConsole();

            Tokenizer = new Tokenizer<TokenType>(Console, TokenType.Invalid, TokenType.Eol, TokenType.Eof);
            Parser = new TokenParser<TokenType, IProcessor>(Console, TokenType.Comment, TokenType.Eol, TokenType.Eof);

            Interpreter = new DslInterpreter<TokenType, IProcessor>(Tokenizer, Parser);

            PropertyProviders = new PropertyProviderList(new BasicFilePropertyProvider());
            IPropertyStore propertyStore = new PropertyStore();
            Pipeline = new ProcessorPipeline(Console, PropertyProviders, propertyStore, Interpreter);

            PluginManager = new PluginManager(this);
            PluginManager.LoadPluginsFromDirectory("plugins");

            cli = new Cli(Console);
            cli.OnCommand += Cli_OnCommand;

            //Temp
            Cli_OnCommand(null, "in test.txt >> eval");
        }

        public void Run() {
            cli.Run();
        }



        private void Cli_OnCommand(object sender, string input) {
            System.Console.Clear();
            List<Token<TokenType>> tokens = Tokenizer.Tokenize(input.Split('\n')).ToList();

            Console.WriteLine("\n----------------------- Tokens ----------------------------\n");


            foreach (Token<TokenType> token in tokens) {
                token.WriteLine(Console, 'a');
            }

            Console.WriteLine("\n----------------------- Parsing ----------------------------\n");

            List<IProcessor> objs = Parser.Parse(tokens);

            Console.WriteLine("\n-------------------- Processor List ----------------------\n");

            foreach (IProcessor a in objs) {
                if (a != null)
                    Console.WriteLine(a.ToString());
            }

            Console.WriteLine("\n--------------------- Pipeline Output --------------------\n");

            Pipeline.Process(objs);
        }


        static int Main(string[] args) {
            Program program = new Program();
            program.Run();
            return 0;
        }

    }

}
