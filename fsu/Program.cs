namespace Maxstupo.Fsu {

    using System;
    using System.Reflection;
    using Maxstupo.Fsu.Core;
    using Maxstupo.Fsu.Core.Utility;
    using Maxstupo.Fsu.Utility;

    public class Program {

        private string Title => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

        private readonly ColorConsole console;

        private readonly FsuEngine fsu;


        public Cli cli;


        public Program() {
            // System.Console.OutputEncoding = Encoding.Unicode;
            System.Console.Title = Title;

            console = new ColorConsole(System.Console.Out);
            fsu = new FsuEngine(console);

            cli = new Cli(console);
            cli.OnCommand += Cli_OnCommand;

            //Temp
            //   if (File.Exists("fsu_on_start.txt"))
            //        Cli_OnCommand(null, File.ReadAllText("fsu_on_start.txt"));
        }

        public void Run() {
            cli.Run();
        }

        private void Cli_OnCommand(object sender, string input) {
            fsu.Evaluate(input);
        }

        [STAThread]
        static int Main(string[] args) {
            Program program = new Program();
            program.Run();


            return 0;
        }

    }

}
