using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxstupo.Fsu.Core.Filtering {

    public class Filter {

        private readonly List<IFilterEntry> conditions = new List<IFilterEntry>();

        public Filter(List<IFilterEntry> conditions) {
            this.conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
        }

        public bool Check(IFilePropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            propertyProvider.Begin();
            bool result = conditions.All(x => x.Evaluate(propertyProvider, propertyStore, item));
            propertyProvider.End();
            return result;
        }

        public class FilterBuilder {
            private readonly List<IFilterEntry> conditions = new List<IFilterEntry>();

            public FilterBuilder Condition(string leftValue, string op, string rightValue) {

                if (string.IsNullOrWhiteSpace(op) || op.Length > 3)
                    throw new ArgumentException(nameof(op));


                Operator ooo = 0;
                if (op.StartsWith("!")) {
                    ooo = Operator.Not;
                    op = op.Substring(1);
                }

                Operator oper;
                if (op[0] == '~' && op.Length == 2 && op[1] == '<') {
                    oper = Operator.EndsWith;
                } else if (op[0] == '<') {
                    oper = Operator.LessThan;
                } else if (op[0] == '>') {
                    oper = Operator.GreaterThan;

                } else if (op[0] == '=') {
                    oper = Operator.Equal;
                } else throw new ArgumentException(nameof(op));

                if (op.Length > 1 && oper != Operator.EndsWith) {
                    char letter = op[1];
                    if (oper == Operator.GreaterThan) {

                        if (letter == '~')
                            oper = Operator.StartsWith;
                        else if (letter == '<')
                            oper = Operator.Contains;

                    } else if (letter != '=' || (letter == '=' && oper == Operator.Equal)) {
                        throw new ArgumentException(nameof(op));
                    } else {
                        oper |= Operator.Equal;
                    }
                }
                oper |= ooo;
                return Condition(leftValue, oper, rightValue);
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
                    throw new Exception();


                conditions.RemoveAt(conditions.Count - 1);
                conditions.Add(new FilterAnd(entry, null));

                return this;
            }

            public FilterBuilder Or() {
                IFilterEntry entry = conditions.LastOrDefault();
                if (entry == null)
                    throw new Exception();

                conditions.RemoveAt(conditions.Count - 1);
                conditions.Add(new FilterOr(entry, null));

                return this;
            }

            public Filter Create() {
                if (conditions.Count != 1)
                    throw new Exception();
                foreach (IFilterEntry entry in conditions) {
                    if (!Check(entry))
                        throw new Exception();
                }
                return new Filter(conditions);
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
