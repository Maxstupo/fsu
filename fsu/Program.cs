using Maxstupo.Fsu.Core;
using Maxstupo.Fsu.Core.Lex;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Maxstupo.Fsu {

    public class Program {

        private Tokenizer tokenizer;
        private ProcessorPipeline pipe;
        private List<ProcessorItem> outputItems;

        public Program(bool silent) {
            Console.Title = Assembly.GetEntryAssembly().GetName().Name;

            string appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FSUtil");
            Directory.CreateDirectory(appDirectory);
            ProcessorManager.Instance.LoadProcessors(silent);

            tokenizer = new Tokenizer();

            foreach (string keyword in ProcessorManager.Instance.Names)
                tokenizer.Keywords.Add(keyword, TokenType.Function);

            pipe = new ProcessorPipeline(silent);
        }

        public void Run() {
            while (true) {
                Console.Write(">> ");
                string input = Console.ReadLine();

                if (input.StartsWith("!")) {
                    ProcessConsoleCommand(input.Trim().Substring(1).ToLowerInvariant());
                } else {
                    Process(input);
                }

            }
        }

        private bool Process(params string[] lines) {
            TokenList list;
            try {
                list = tokenizer.Parse(lines);
            } catch (Exception) {
                ColorConsole.WriteLine("&-c;Syntax error: missing double quote.&-^;");
                return false;
            }

            if (TokenParser.Parse(pipe, list, out List<string> startingItems)) {
                outputItems = pipe.Run(startingItems.ToArray());
                return true;
            }
            return false;
        }

        private void ProcessConsoleCommand(string input) {
            string[] tokens = input.Split(' ');
            switch (tokens[0]) {
                case "clear":
                case "cls":
                    outputItems = null;
                    Console.Clear();
                    break;
                case "exit":
                case "quit":
                    Environment.Exit(0);
                    break;
                case "open":
                    if (outputItems != null && outputItems.Count > 0) {
                        if (tokens.Length > 1 && int.TryParse(tokens[1], out int index)) {
                            if (!(index < 0 || index >= outputItems.Count)) {
                                string filepath = outputItems[index].Value;
                                if (!File.Exists(filepath))
                                    filepath = outputItems[index].InitalValue;
                                if (File.Exists(filepath))
                                    System.Diagnostics.Process.Start(filepath);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        [STAThread]
        static int Main(string[] args) {
            bool silent = args.Length > 1 && args[1].Equals("-q", StringComparison.InvariantCultureIgnoreCase);
            Program program = new Program(silent);

            if (args.Length > 0)
                return program.Process(args[0]) ? 0 : -1;
            else
                program.Run();
            return 0;
        }

    }

}
