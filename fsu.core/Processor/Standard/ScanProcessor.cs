using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Maxstupo.Fsu.Core.Processor.Standard {
    public class ScanProcessor : IProcessor {

        private readonly bool isFiles;
        private readonly SearchOption searchOption;

        public ScanProcessor(bool isFiles = true, SearchOption searchOption = SearchOption.AllDirectories) {
            this.isFiles = isFiles;
            this.searchOption = searchOption;
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {

            IEnumerable<ProcessorItem> result = Enumerable.Empty<ProcessorItem>();

            foreach (ProcessorItem item in items) {
                IEnumerable<string> enumerable = isFiles ?
                                                         GetDirectoryFiles(item.Value, "*", searchOption)
                                                         :
                                                         GetDirectoryFiles(item.Value, "*", searchOption);

                result = result.Concat(enumerable.Select(x => new ProcessorItem(x)));
            }

            return result;
        }



        public static IEnumerable<string> GetDirectoryFiles(string rootPath, string patternMatch, SearchOption searchOption) {
            IEnumerable<string> foundFiles = Enumerable.Empty<string>();

            if (searchOption == SearchOption.AllDirectories) {
                try {
                    IEnumerable<string> subDirs = Directory.EnumerateDirectories(rootPath);
                    foreach (string dir in subDirs)
                        foundFiles = foundFiles.Concat(GetDirectoryFiles(dir, patternMatch, searchOption)); // Add files in subdirectories recursively to the list

                } catch (UnauthorizedAccessException e) {
                    Console.WriteLine(e.ToString());
                } catch (PathTooLongException) {
                }
            }

            try {
                foundFiles = foundFiles.Concat(Directory.EnumerateFiles(rootPath, patternMatch)); // Add files from the current directory
            } catch (UnauthorizedAccessException e) {
                Console.WriteLine("AAAA: " + e.ToString());
            }

            return foundFiles;
        }

    }
}
