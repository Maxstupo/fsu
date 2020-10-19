namespace Maxstupo.Fsu.Core.Filtering {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public class Filter {

        private readonly List<IFilterEntry> conditions;

        public Filter(List<IFilterEntry> conditions) {
            this.conditions = new List<IFilterEntry>(conditions);
        }

        public bool Check(IOutput console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            propertyProvider.Begin();

            bool result = conditions.All(x => x.Evaluate(console, propertyProvider, propertyStore, item));

            propertyProvider.End();

            return result;
        }

        public class FilterBuilder {

            private readonly List<IFilterEntry> conditions = new List<IFilterEntry>();

            private IFilterEntry left;
            private bool isAnd;
            private bool isOr;
                       
            public FilterBuilder Condition(Operand leftValue, Operator op, Operand rightValue) {

                FilterCondition condition = new FilterCondition(leftValue, op, rightValue);

                if (isAnd) {
                    conditions.Add(new FilterAnd(left, condition));
                    isAnd = false;

                } else if (isOr) {
                    conditions.Add(new FilterOr(left, condition));
                    isOr = false;

                } else {
                    conditions.Add(condition);

                }

                return this;
            }

            public FilterBuilder And() {
                if (isAnd)
                    throw new InvalidOperationException("Cant start another AND filter until previous AND filter has right-side condition.");

                IFilterEntry entry = conditions.LastOrDefault();
                left = entry ?? throw new Exception("Can't use a And condition on an empty filter.");
                conditions.RemoveAt(conditions.Count - 1);
                isAnd = true;
                return this;
            }

            public FilterBuilder Or() {
                if (isOr)
                    throw new InvalidOperationException("Cant start another OR filter until previous OR filter has right-side condition.");

                IFilterEntry entry = conditions.LastOrDefault();
                left = entry ?? throw new Exception("Can't use a Or condition on an empty filter.");
                conditions.RemoveAt(conditions.Count - 1);
                isOr = true;
                return this;
            }

            public Filter Create() {
                //  if (conditions.Count != 1)
                //       throw new Exception();
                foreach (IFilterEntry entry in conditions) {
                    if (!Check(entry))
                        throw new Exception("Filter missing conditions for AND/OR condition.");
                }
                Filter filter = new Filter(conditions);
                conditions.Clear();
                return filter;
            }

            private bool Check(IFilterEntry entry) {
                if (entry is FilterCondition)
                    return true;

                if (entry is FilterAnd and) {
                    if (and.Left == null || and.Right == null)
                        return false;

                    return Check(and.Left) && Check(and.Right);

                } else if (entry is FilterOr or) {
                    if (or.Left == null || or.Right == null)
                        return false;

                    return Check(or.Left) && Check(or.Right);
                }

                return true;
            }

        }

        public static FilterBuilder Builder() {
            return new FilterBuilder();
        }

    }

}