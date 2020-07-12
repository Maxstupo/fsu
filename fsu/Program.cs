namespace Maxstupo.Fsu {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Maxstupo.Fsu.CommandTree;
    using Maxstupo.Fsu.CommandTree.Attributes;
    using Maxstupo.Fsu.CommandTree.Parameters;
    using Maxstupo.Fsu.CommandTree.Providers;
    using Maxstupo.Fsu.Core;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using Maxstupo.Fsu.Providers;
    using Maxstupo.Fsu.Utility;

    public class Program {

        private string Title => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

        private readonly ColorConsole console;

        private readonly FsuEngine fsu;


        public Cli cli;
        private readonly CommandInterpreter commandLine;

        private IEnumerable<ProcessorItem> results;

        public Program() {

            System.Console.OutputEncoding = Encoding.Unicode;
            System.Console.Title = Title;

            console = new ColorConsole(System.Console.Out);
            console.Level = Level.Debug;

            fsu = new FsuEngine(console);
            fsu.PropertyProviders.Add(new ExtendedFilePropertyProvider());
            fsu.FallbackItems = new string[] { Directory.GetCurrentDirectory() };

            cli = new Cli(console);
            cli.OnCommand += Cli_OnCommand;

            commandLine = new CommandInterpreter(console, true);
            commandLine.MessageProvider.Set(StandardMessages.NoCommand, "No command called '&-a;{0}&-^;' was found.");
            commandLine.MessageProvider.Set(StandardMessages.NextHelpPageTip, "Type '!help {0}( {1})' to read the next page.");

            InitCommands();

            //Temp
            if (File.Exists("fsu_on_start.txt"))
                Cli_OnCommand(null, File.ReadAllText("fsu_on_start.txt"));
        }

        private void InitCommands() {
            Command cmdOpen = new Command("Open", "open", description: "Opens a result with the default application.");
            cmdOpen.Parameters.Add(new ParamDef("index", null, typeof(int), description: "The index of the item to open."));
            cmdOpen.OnExecuted += OpenResult;

            commandLine.Register(cmdOpen);

            Command cmdClear = new Command("Clear", "clear", Aliases.Create("cls"), "Clears the window.");
            cmdClear.OnExecuted += data => console.Clear();

            commandLine.Register(cmdClear);
        }

        private void OpenResult(CommandData data) {
            int index = data.Parameters.Get<int>(0);

            List<ProcessorItem> items = results.ToList();
            if (index < 0 || index >= items.Count)
                return;

            ProcessorItem item = items[index];

            if (File.Exists(item.Value))
                Process.Start(item.Value);
        }

        public void Run() {
            cli.Run();
        }

        private void Cli_OnCommand(object sender, string input) {
            if (input.StartsWith("!")) {
                commandLine.Process(input.Substring(1));
            } else {
                results = fsu.Evaluate(input);
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
