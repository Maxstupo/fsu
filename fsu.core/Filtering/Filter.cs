using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Core.Filtering {

    public class Filter {

        private readonly List<IFilterEntry> conditions;

        public Filter(List<IFilterEntry> conditions) {
            this.conditions = new List<IFilterEntry>(conditions);
        }

        public bool Check(IConsole console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            propertyProvider.Begin();

            bool result = conditions.All(x => x.Evaluate(console, propertyProvider, propertyStore, item));

            propertyProvider.End();

            return result;
        }

        public class FilterBuilder {

            private readonly List<IFilterEntry> conditions = new List<IFilterEntry>();

            public FilterBuilder Condition(string leftValue, string op, string rightValue) {
                Operator operatorValue = 0;
                string opValue = op.ToLowerInvariant();

                if (opValue.StartsWith("!")) {
                    opValue = opValue.Substring(1);
                    operatorValue |= Operator.Not;
                }


                if (opValue == "<>") {
                    operatorValue |= Operator.Regex; 
                } else if (opValue == "><") {
                    operatorValue |= Operator.Contains;
                } else if (opValue == "~<") {
                    operatorValue |= Operator.EndsWith;
                } else if (opValue == ">~") {
                    operatorValue |= Operator.StartsWith;
                } else {

                    if (opValue.Contains("="))
                        operatorValue |= Operator.Equal;

                    if (opValue.Contains("<"))
                        operatorValue |= Operator.LessThan;
                    else if (opValue.Contains(">"))
                        operatorValue |= Operator.GreaterThan;
                }

                return Condition(leftValue, operatorValue, rightValue);
            }

            public FilterBuilder Condition(string leftValue, Operator op, string rightValue) {
                FilterCondition condition = new FilterCondition(leftValue, op, rightValue);

                IFilterEntry entry = conditions.LastOrDefault();
                if (entry != null) {
                    if (entry is FilterAnd and && and.Right == null) {
                        and.Right = condition;
                    } else if (entry is FilterOr or && or.Right == null) {
                        or.Right = condition;
                    } else {
                        conditions.Add(condition);
                    }
                } else {
                    conditions.Add(condition);
                }

                return this;
            }

            public FilterBuilder And() {
                IFilterEntry entry = conditions.LastOrDefault();
                if (entry == null)
                    throw new Exception("Can't use a And condition on an empty filter.");


                conditions.RemoveAt(conditions.Count - 1);
                conditions.Add(new FilterAnd(entry, null));

                return this;
            }

            public FilterBuilder Or() {
                IFilterEntry entry = conditions.LastOrDefault();
                if (entry == null)
                    throw new Exception("Can't use a Or condition on an empty filter.");

                conditions.RemoveAt(conditions.Count - 1);
                conditions.Add(new FilterOr(entry, null));

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
