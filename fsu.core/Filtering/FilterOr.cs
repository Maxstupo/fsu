using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;

namespace Maxstupo.Fsu.Core.Filtering {

    public class FilterOr : IFilterEntry {

        public IFilterEntry Left { get; set; }

        public IFilterEntry Right { get; set; }

        public FilterOr(IFilterEntry left, IFilterEntry right) {
            Left = left;
            Right = right;
        }

        public bool Evaluate(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            return Left.Evaluate(propertyProvider, propertyStore, item) || Right.Evaluate(propertyProvider, propertyStore, item);
        }

    }

}
