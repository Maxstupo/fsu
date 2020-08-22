namespace Maxstupo.Fsu.Core.Processor.Processors {

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public class ExecProcessor : IProcessor {

        private readonly FormatTemplate exeTemplate;
        private readonly FormatTemplate argsTemplate;

        private readonly bool noWindow;
        private readonly bool wait;

        public ExecProcessor(FormatTemplate execTemplate, FormatTemplate argumentsTemplate, bool noWindow, bool wait) {
            this.exeTemplate = execTemplate;
            this.argsTemplate = argumentsTemplate;
            this.noWindow = noWindow;
            this.wait = wait;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo() {
                CreateNoWindow = noWindow,
                ErrorDialog = false,
                UseShellExecute = !noWindow,
                RedirectStandardOutput = noWindow,
                RedirectStandardError = noWindow,
                StandardOutputEncoding = Encoding.Unicode,
                StandardErrorEncoding = Encoding.Unicode
            }; 
            
            if (pipeline.Simulate)
                pipeline.Output.WriteLine(Utility.Level.Info, $"\n&-c;-------- Simulation Mode Active! --------&-^;\n");


            foreach (ProcessorItem item in items) {

                string exeFilepath = exeTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);


                string exeArguments = argsTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);


                info.FileName = exeFilepath;
                info.Arguments = exeArguments;


                System.Diagnostics.Process process = !pipeline.Simulate ? new System.Diagnostics.Process { StartInfo = info } : null;

                if (noWindow && !pipeline.Simulate) {
                    process.OutputDataReceived += (p, data) => pipeline.Output.WriteLine(Level.None, data.Data, true);
                    process.ErrorDataReceived += (p, data) => pipeline.Output.WriteLine(Level.None, data.Data, true);
                }

                try {
                    process?.Start();

                    pipeline.Output.WriteLine(Level.None, $"Executing: {exeFilepath} {exeArguments}");

                    if (noWindow) {
                        process?.BeginErrorReadLine();
                        process?.BeginOutputReadLine();
                    }

                    if (wait)
                        process?.WaitForExit();

                } catch (Win32Exception) {
                    pipeline.Output.WriteLine(Level.Error, $"&-c;Failed to execute: {exeFilepath} {exeArguments}&-^;");
                }
            }

            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}[exec='{exeTemplate.Template}', args='{argsTemplate.Template}']";
        }

    }
}
