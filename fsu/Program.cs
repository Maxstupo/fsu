namespace Maxstupo.Fsu {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using CommandLine;
    using CommandLine.Text;
    using Maxstupo.Fsu.CommandTree;
    using Maxstupo.Fsu.CommandTree.Parameters;
    using Maxstupo.Fsu.CommandTree.Providers;
    using Maxstupo.Fsu.Core;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using Maxstupo.Fsu.Providers;
    using Maxstupo.Fsu.Utility;

    public class Program {

        private string Title => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

        private ColorConsole console;
        private FsuEngine fsu;


        public Cli cli;
        private CommandInterpreter commandLine;

        private IEnumerable<ProcessorItem> results;

        public Program() {
            System.Console.OutputEncoding = Encoding.Unicode;
            System.Console.Title = Title;
        }

        private void Init(Options options) {
            console = new ColorConsole(System.Console.Out) { Level = options.Level };

            fsu = new FsuEngine(console);
            fsu.Pipeline.Simulate = true;
            fsu.PropertyProviders.Add(new ExtendedFilePropertyProvider());

            string[] fallbackItems = options.FallbackItems.ToArray();
            fsu.FallbackItems = fallbackItems.Length > 0 ? fallbackItems : new string[] { Directory.GetCurrentDirectory() };

            cli = new Cli(console);
            cli.OnCommand += Cli_OnCommand;

            commandLine = new CommandInterpreter(console, true);
            commandLine.MessageProvider.Set(StandardMessages.NoCommand, "No command called '&-a;{0}&-^;' was found.");
            commandLine.MessageProvider.Set(StandardMessages.NextHelpPageTip, "Type '!help {0}( {1})' to read the next page.");

            InitCommands();
        }

        private void InitCommands() {
            Command cmdOpen = new Command("Open", "open", description: "Opens a result with the default application.");
            cmdOpen.Parameters.Add(new ParamDef("index", null, typeof(int), description: "The index of the item to open."));
            cmdOpen.OnExecuted += OpenResult;

            commandLine.Register(cmdOpen);

            Command cmdClear = new Command("Clear", "clear", Aliases.Create("cls"), "Clears the window.");
            cmdClear.OnExecuted += data => console.Clear();

            commandLine.Register(cmdClear);

            Command cmdExit = new Command("Exit", "exit", Aliases.Create("q", "quit", "stop"), "Exits the fsu shell.");
            cmdExit.OnExecuted += data => cli.IsRunning = false;

            commandLine.Register(cmdExit);

            Command cmSim = new Command("Simulate", "sim", null, "Toggle simulation mode.");
            cmSim.OnExecuted += data => {
                fsu.Pipeline.Simulate = !fsu.Pipeline.Simulate;
                data.Output.WriteLine(Level.Info, $"-- Simulation mode is {(fsu.Pipeline.Simulate ? "on" : "off")}");
            };
            commandLine.Register(cmSim);
        }

        private int Start(Options options) {
            if (options.FileToEvaluate != null && File.Exists(options.FileToEvaluate))
                Cli_OnCommand(null, File.ReadAllText(options.FileToEvaluate));

            if (!options.NoPrompt)
                Run();

            return 0;
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

#if DEBUG
            if (Debugger.IsAttached)
                args = "-l Fine -f fsu_on_start.txt".Split(' ');
#endif

            Parser parser = new Parser(with => { with.HelpWriter = null; with.CaseInsensitiveEnumValues = true; });
            ParserResult<Options> result = parser.ParseArguments<Options>(args);

            int exitCode = 0;

            Program program = new Program();

            result
                .WithParsed(options => { program.Init(options); exitCode = program.Start(options); })
                .WithNotParsed(errs => {
                    DisplayHelp(result, errs);
                    exitCode = -1;
                });

#if DEBUG
            if (Debugger.IsAttached) {
                Console.Write("Press any key to continue...");
                Console.ReadLine();
            }
#endif
            return exitCode;
        }

        static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs) {
            HelpText helpText = HelpText.AutoBuild(result, h => {

                h.AdditionalNewLineAfterOption = false;
                h.AddEnumValuesToHelpText = true;

                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);

            Console.WriteLine(helpText);
        }


    }

}