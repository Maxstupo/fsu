namespace Maxstupo.Fsu.Core.Filtering {

    using System;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public class FilterOr : IFilterEntry {

        public IFilterEntry Left { get; }

        public IFilterEntry Right { get; }

        public FilterOr(IFilterEntry left, IFilterEntry right) {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public bool Evaluate(IOutput console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            return Left.Evaluate(console, propertyProvider, propertyStore, item) || Right.Evaluate(console, propertyProvider, propertyStore, item);
        }

    }

}