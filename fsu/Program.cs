using GlobExpressions;
using Maxstupo.Fsu.Core;
using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Processor.Standard;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Maxstupo.Fsu {

    public enum TokenType {
        Invalid,
        Eol,
        Eof,

        [TokenDef(">>", HasVariableValue = false)]
        Pipe,

        //  [TokenDef("scan|transform|print|filter|in|out|glob")]
        [TokenDef("scan")]
        [TokenDef("transform")]
        [TokenDef("print")]
        [TokenDef("filter")]
        [TokenDef("in")]
        [TokenDef("out")]
        [TokenDef("glob")]
        Function,

        [TokenDef(@"(?:\<|\>)=?", 2)]
        NumericOperator,

        [TokenDef(@"\>\<|\<\>")]
        [TokenDef(@"~\<|\>~")]
        StringOperator,



        [TokenDef(@"!", HasVariableValue = false)]
        Not,

        [TokenDef(@"&|\|")]
        LogicOperator,

        [TokenDef("files|dirs|dir|file|top")]
        Mode,

        [TokenDef("\\\"([^\"]*)\\\"")]
        StringValue,

        [TokenDef(@"\@\w+")]
        ItemProperty,

        [TokenDef(@"\$\w+")]
        GlobalProperty,

        [TokenDef(@"-?(\d+)\.(\d+)?", Template ="{1}apple{2} ({0})")]
        NumberValue,

        [TokenDef(@"//")]
        Comment,
        [TokenDef("mb|gb|tb|kb|s|m|h|d|y")]
        Unit,

        [TokenDef(@"[\w\d:\\/\.\-]+", 3)]
        TextValue,
    }

    public class Program {

        private readonly Cli cli = new Cli();
        private readonly IProcessorPipeline pipeline;

        private readonly ITokenizer<TokenType> tokenizer = new Tokenizer<TokenType>(TokenType.Invalid, TokenType.Eol, TokenType.Eof);

        public Program() {
            Console.Title = Assembly.GetEntryAssembly().GetName().Name;


            cli.OnCommand += Cli_OnCommand;
            cli.OnAutoComplete += Cli_OnAutoComplete;


            IPropertyProvider propertyProvider = new BasicFilePropertyProvider();
            IPropertyStore propertyStore = new PropertyStore();

            pipeline = new ProcessorPipeline(propertyProvider, propertyStore);


            List<IProcessor> list = new List<IProcessor> {
              //  new InProcessor("test.txt"),
                //new ScanProcessor(true, SearchOption.AllDirectories),
                new GlobProcessor("*.{pdf,stl,txt}"),
              //   new OutProcessor("test.txt"),
                new PrintProcessor(),
            };

            List<ProcessorItem> items = new List<ProcessorItem>() {
                new ProcessorItem(@"E:\Videos")
            };

            //     pipeline.Process(list, items);
            Cli_OnCommand(this, "");
        }

        public void Run() {
            cli.Run();
        }

        private string Cli_OnAutoComplete(string text, int caretIndex) {
            string value = text.Substring(0, caretIndex).Trim().ToLowerInvariant();

            if (value.EndsWith("scan"))
                return " files";

            return null;
        }

        private void Cli_OnCommand(object sender, string input) {
            ColorConsole.WriteLine(input);

            if (input.StartsWith("!")) {

                string[] tokens = input.Substring(1).ToLowerInvariant().Split(' ');
                switch (tokens[0]) {
                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;
                    case "hello":
                        Console.WriteLine("Hello World");
                        break;
                    case "exit":
                    case "quit":
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            } else {

                Console.Clear();

                List<Token<TokenType>> tokens = tokenizer.Parse(File.ReadAllLines("test.txt")).ToList();

                // IEnumerable<Token<TokenType>> tokenMatches = ((Tokenizer<TokenType>) tokenizer).FindAllTokenMatches(File.ReadAllText("test.txt"), 1);

                //  IEnumerable<IGrouping<int, Token<TokenType>>> groupedByStartIndex = tokenMatches.GroupBy(x => x.StartIndex).OrderBy(x => x.Key);

                Console.WriteLine();

                //foreach (var a in groupedByStartIndex) {
                //    foreach (var item in a.OrderBy(x => x.Precedence)) {

                //        //      item.Print();
                //    }
                //    // Console.WriteLine("----------------------------------------");

                //}

                bool comment = false;
                foreach (var a in tokens) {
                    if ((!comment && a.TokenType == TokenType.Comment))
                        comment = true;

                    a.Print(comment ? '7' : 'a');
                    if ((comment && a.TokenType == TokenType.Eol))
                        comment = false;
                }
            }

        }

        [STAThread]
        static int Main(string[] args) {
            Program program = new Program();
            program.Run();
            return 0;
        }

    }

}
