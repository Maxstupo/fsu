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
    using Maxstupo.Fsu.Core.Detail;
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
            System.Console.OutputEncoding = Encoding.UTF8;
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

            Command cmSim = new Command("Simulate", "simulate", Aliases.Create("sim"), "Enable/disable simulation mode.");
            cmSim.Parameters.Add(new ParamDef("value", string.Empty, typeof(string), "True to enable simulation mode, leave blank for toggle"));
            cmSim.OnExecuted += data => {

                string value = data.Parameters.Get<string>("value");
                if (string.IsNullOrWhiteSpace(value)) {
                    fsu.Pipeline.Simulate = !fsu.Pipeline.Simulate;

                } else if (bool.TryParse(value, out bool simulate)) {
                    fsu.Pipeline.Simulate = simulate;

                }
                data.Output.WriteLine(Level.Info, $"Simulation mode is {(fsu.Pipeline.Simulate ? "&-a;on" : "&-c;off")}&-^;");
            };
            commandLine.Register(cmSim);

            Command cmdPersistent = new Command("Persistent", "persistent", Aliases.Create("p", "pstore"), "Enable/disable persistent property store.");
            cmdPersistent.Parameters.Add(new ParamDef("value", string.Empty, typeof(string), "True to enable persistent mode, leave blank for toggle"));
            cmdPersistent.OnExecuted += data => {

                string value = data.Parameters.Get<string>("value");
                if (string.IsNullOrWhiteSpace(value)) {
                    fsu.PersistentStore = !fsu.PersistentStore;

                } else if (bool.TryParse(value, out bool persistentStore)) {
                    fsu.PersistentStore = persistentStore;

                }
                data.Output.WriteLine(Level.Info, $"Persistent store mode is {(fsu.PersistentStore ? "&-a;on" : "&-c;off")}&-^;");
            };
            commandLine.Register(cmdPersistent);

            Command cmdPurge = new Command("Purge", "purge", Aliases.Create("clean"), "Clears the property store.");
            cmdPurge.OnExecuted += data => {
                fsu.PropertyStore.ClearAll();
                data.Output.WriteLine(Level.None, "&-c;Property store purged!&-^;");
            };
            commandLine.Register(cmdPurge);

            Command cmdStore = new Command("Store", "store", Aliases.Create("summary"), "Displays a summary of properties within the property store.");
            cmdStore.OnExecuted += data => {
                if (fsu.PropertyStore.Count == 0) {
                    data.Output.WriteLine(Level.None, "&-c;Property store is empty!&-^;");
                } else {
                    data.Output.WriteLine(Level.None, "\n------------- Property Store Summary --------------");
                    foreach (KeyValuePair<string, PropertyItem> item in fsu.PropertyStore)
                        data.Output.WriteLine(Level.None, $"{item.Key}: {item.Value.Value}");
                }
            };
            commandLine.Register(cmdStore);
        }

        private int Start(Options options) {
            bool hasSpecificScriptFile = options.FileToEvaluate != null && File.Exists(options.FileToEvaluate);

            // Run a script.
            if (!string.IsNullOrWhiteSpace(options.ScriptName) || hasSpecificScriptFile) {
                if (!hasSpecificScriptFile)
                    console.WriteLine(Level.Info, $"Running script: &-6;{options.ScriptName}&-^;");

                string filename = hasSpecificScriptFile ? Path.GetFileNameWithoutExtension(options.FileToEvaluate) : $"{options.ScriptName}.fsu";
                string filepath = hasSpecificScriptFile ? Path.GetFullPath(options.FileToEvaluate) : Util.GetFirstExistingFile(Path.Combine(Directory.GetCurrentDirectory(), filename), Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename));

                if (filepath != null && File.Exists(filepath)) {
                    console.WriteLine(Level.Debug, $"Executing script: &-6;{filepath}&-^;");

                    fsu.PropertyStore.SetProperty("_filepath", new PropertyItem(filepath), true);
                    fsu.PropertyStore.SetProperty("_filename", new PropertyItem(filename), true);
                    fsu.PropertyStore.SetProperty("_name", new PropertyItem(options.ScriptName), true);

                    int i = 0;
                    foreach (string arg in options.ScriptParams)
                        fsu.PropertyStore.SetProperty($"_{i++}", new PropertyItem(arg), true);


                    foreach (string line in File.ReadLines(filepath)) {
                        Cli_OnCommand(null, line);
                    }

                } else {
                    console.WriteLine(Level.Warn, $"&-c;No script found!&-^;");
                    return -1;
                }

            } else {
                Run();
            }

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
                args = "vid 12 -l Fine".Split(' ');
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