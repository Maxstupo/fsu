﻿using Maxstupo.Fsu.Core.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maxstupo.Fsu.Processors {
    public class OutProcessor : IProcessor {

        private readonly string path;

        public OutProcessor(string path) {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public IEnumerable<ProcessorItem> Process(IProcessorPipeline pipeline, IEnumerable<ProcessorItem> items) {
            File.WriteAllLines(path, items.Select(x => x.Value), Encoding.UTF8);
            return items;
        }

        public override string ToString() {
            return $"{GetType().Name}[path='{path}']";
        }
    }
}