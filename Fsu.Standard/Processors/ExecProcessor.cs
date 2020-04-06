using Maxstupo.Fsu.Core.Format;
using Maxstupo.Fsu.Core.Processor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Maxstupo.Fsu.Standard.Processor {
    public class ExecProcessor : IProcessor {

        private readonly FormatTemplate exeTemplate;
        private readonly FormatTemplate argsTemplate;

        private readonly bool noWindow;

        public ExecProcessor(FormatTemplate execTemplate, FormatTemplate argumentsTemplate, bool noWindow) {
            this.exeTemplate = execTemplate;
            this.argsTemplate = argumentsTemplate;
            this.noWindow = noWindow;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo() {
                CreateNoWindow = noWindow,
                ErrorDialog = false,
                UseShellExecute = !noWindow,
                RedirectStandardOutput = noWindow,
                RedirectStandardError = noWindow
            };

            foreach (ProcessorItem item in items) {

                string exeFilepath = exeTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);


                string exeArguments = argsTemplate.Make(pipeline.PropertyProvider, pipeline.PropertyStore, item);

            
                info.FileName = exeFilepath;
                info.Arguments = exeArguments;

                System.Diagnostics.Process process = new System.Diagnostics.Process { StartInfo = info };

                if (noWindow) {
                    process.OutputDataReceived += (p, data) => pipeline.Console.WriteLine(data.Data, true);
                    process.ErrorDataReceived += (p, data) => pipeline.Console.WriteLine(data.Data, true);

                }

                try {
                    process.Start();

                    pipeline.Console.WriteLine($"Exec: {exeFilepath} {exeArguments}");

                    if (noWindow) {
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                    }
                } catch (Win32Exception) {
                    pipeline.Console.WriteLine($"&-c;Exec failed: {exeFilepath} {exeArguments}&-^;");
                }

                yield return item;
            }
        }

        public override string ToString() {
            return $"{GetType().Name}[exec='{exeTemplate.Template}', args='{argsTemplate.Template}']";
        }

    }
}
