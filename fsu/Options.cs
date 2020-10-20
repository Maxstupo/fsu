﻿namespace Maxstupo.Fsu {

    using System.Collections.Generic;
    using CommandLine;
    using Maxstupo.Fsu.Core.Utility;

    public class Options {

        [Option('l', "level", Default = Level.Info, HelpText = "The logging level for fsu.")]
        public Level Level { get; set; }

        [Option('f', "file", Default = null, HelpText = "The file to evaluate at startup.")]
        public string FileToEvaluate { get; set; }

        [Option('n', "no-prompt", HelpText = "Automatically exits the shell after startup, useful for evaluating files and then closing")]
        public bool NoPrompt { get; set; }

        [Option("fallback-items", Separator = ',', HelpText = "Processor stream input items to use when nothing is initially provided. Defaults to current directory.")]
        public IEnumerable<string> FallbackItems { get; set; }

    }

}