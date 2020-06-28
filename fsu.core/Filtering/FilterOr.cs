namespace Maxstupo.Fsu.Core.Filtering {

    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using System;

    public class FilterOr : IFilterEntry {

        public IFilterEntry Left { get; set; }

        public IFilterEntry Right { get; set; }

        public FilterOr(IFilterEntry left, IFilterEntry right) {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right;
        }

        public bool Evaluate(IConsole console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            return Left.Evaluate(console, propertyProvider, propertyStore, item) || Right.Evaluate(console, propertyProvider, propertyStore, item);
        }

    }

}