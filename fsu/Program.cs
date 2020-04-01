using Maxstupo.Fsu.Core.Utility;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Maxstupo.Fsu {

    public class Program {

        private readonly Cli cli = new Cli();

        public Program() {
            Console.Title = Assembly.GetEntryAssembly().GetName().Name;

            cli.OnCommand += Cli_OnCommand;
            cli.OnAutoComplete += Cli_OnAutoComplete;
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

            string[] tokens = input.ToLowerInvariant().Split(' ');
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
        }

        [STAThread]
        static int Main(string[] args) {
            Program program = new Program();
            program.Run();
            return 0;
        }

    }

}
