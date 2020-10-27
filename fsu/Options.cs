namespace Maxstupo.Fsu {

    using System.Collections.Generic;
    using CommandLine;
    using Maxstupo.Fsu.Core.Utility;

    public class Options {

        [Option('l', "level", Default = Level.Info, HelpText = "The logging level for fsu.")]
        public Level Level { get; set; }

        [Option('f', "file", Default = null, HelpText = "The script filepath to execute. Overrides 'script-name' option.")]
        public string FileToEvaluate { get; set; }

        [Option("fallback-items", Separator = ',', HelpText = "Processor stream input items to use when nothing is initially provided. Defaults to current directory.")]
        public IEnumerable<string> FallbackItems { get; set; }

        [Value(0, Required = false, MetaName = "script-name", HelpText = "The script filename (without extension) to execute.")]
        public string ScriptName { get; set; }

        [Value(1, Required = false, MetaName = "script-params", HelpText = "The script parameters.")]
        public IEnumerable<string> ScriptParams { get; set; }

    }

}