namespace Maxstupo.Fsu.Core.Filtering {

    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Format;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class FilterCondition : IFilterEntry {
        private static readonly Regex regex = new Regex(@"^(\d+(?:\.\d+)?)\s*([a-z]{1,2})?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public string Left { get; }
        private PropertyItem propLeft;
        private readonly bool isLeftItemProperty;
        private readonly bool isLeftGlobalProperty;

        public Operator Operator { get; }

        public string Right { get; }
        private PropertyItem propRight;
        private readonly bool isRightItemProperty;
        private readonly bool isRightGlobalProperty;

        private bool ignored = false;


        public FilterCondition(string leftValue, Operator op, string rightValue) {
            Left = leftValue ?? throw new ArgumentNullException(nameof(leftValue));
            isLeftItemProperty = Left.StartsWith("@");
            isLeftGlobalProperty = Left.StartsWith("$");
            if (isLeftItemProperty || isLeftGlobalProperty)
                Left = Left.Substring(1);

            Operator = op;

            Right = rightValue ?? throw new ArgumentNullException(nameof(rightValue));
            isRightItemProperty = Right.StartsWith("@");
            isRightGlobalProperty = Right.StartsWith("$");
            if (isRightItemProperty || isRightGlobalProperty)
                Right = Right.Substring(1);
        }

        public bool Evaluate(IConsole console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {
            if (ignored)
                return false;

            PropertyItem valueLeft = GetValue(console, propertyProvider, propertyStore, item, true);
            if (valueLeft == null)
                return false;


            PropertyItem valueRight = GetValue(console, propertyProvider, propertyStore, item, false);
            if (valueRight == null)
                return false;


            if (valueLeft.IsNumeric && valueRight.IsNumeric) { // If we are comparing two numbers.

                if (Operator.HasFlag(Operator.StartsWith) || Operator.HasFlag(Operator.EndsWith) || Operator.HasFlag(Operator.Contains) || Operator.HasFlag(Operator.Regex)) {
                    ignored = true;
                    console.WriteLine($"&-c;Unsupported operator for numeric comparison &-e;'{Operator}'&-^;, unable to compare values, ignoring condition...&-^;");
                    return true;
                }

                bool result = false;

                double vl = valueLeft.ValueNumber;
                double vr = valueRight.ValueNumber;

                if (Operator.HasFlag(Operator.LessThan)) {
                    result = vl < vr;
                } else if (Operator.HasFlag(Operator.GreaterThan)) {
                    result = vl > vr;
                }

                if (Operator.HasFlag(Operator.Equal) && !result)
                    result = vl == vr;

                return result == !Operator.HasFlag(Operator.Not);

            } else if (!valueLeft.IsNumeric && !valueRight.IsNumeric) { // If we are comparing two strings.

                string vl = valueLeft.ValueText;
                string vr = valueRight.ValueText;

                if (Operator.HasFlag(Operator.StartsWith)) {
                    return vl.StartsWith(vr, StringComparison.InvariantCultureIgnoreCase) == !Operator.HasFlag(Operator.Not);

                } else if (Operator.HasFlag(Operator.EndsWith)) {
                    return vl.EndsWith(vr, StringComparison.InvariantCultureIgnoreCase) == !Operator.HasFlag(Operator.Not);

                } else if (Operator.HasFlag(Operator.Equal)) {
                    return vl.Equals(vr, StringComparison.InvariantCultureIgnoreCase) == !Operator.HasFlag(Operator.Not);

                } else if (Operator.HasFlag(Operator.Contains)) {
                    return vl.Contains(vr, StringComparison.InvariantCultureIgnoreCase) == !Operator.HasFlag(Operator.Not);

                } else if (Operator.HasFlag(Operator.Regex)) {
                    //return vl.Contains(vr, StringComparison.InvariantCultureIgnoreCase) == !Operator.HasFlag(Operator.Not);
                    return Regex.IsMatch(vl, vr, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) == !Operator.HasFlag(Operator.Not);
                } else {
                    ignored = true;
                    console.WriteLine($"&-c;Unsupported operator for string comparison &-e;'{Operator}'&-^;, unable to compare values, ignoring condition...&-^;");
                    return true;
                }

            } else {
                ignored = true;
                console.WriteLine($"&-c;Condition type mismatch, unable to compare values, ignoring condition...&-^;");
                return true;
            }

        }


        private PropertyItem GetValue(IConsole console, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item, bool isLeft) {
            if (isLeft && propLeft != null)
                return propLeft;
            else if (!isLeft && propRight != null)
                return propRight;

            bool isGlobal = isLeft ? isLeftGlobalProperty : isRightGlobalProperty;
            bool isItem = isLeft ? isLeftItemProperty : isRightItemProperty;

            string text = isLeft ? Left : Right;

            if (isItem) { // it's an item property, needs to be collected for each item.
                PropertyItem property = item.GetProperty(propertyProvider, text);
                if (property != null)
                    return property;

                //          console.WriteLine($"  &-c;Item property named '&-e;{text}&-^;' doesn't exist or isn't numeric, ignoring condition...&-^;");
                return null;

            } else if (isGlobal) {
                PropertyItem property = propertyStore.GetProperty(text);
                if (property != null) {
                    if (isLeft) propLeft = property; else propRight = property;
                    return property;
                }
                ignored = true;
                console.WriteLine($"  &-c;Global property named '&-e;{text}&-^;' doesn't exist or isn't numeric, ignoring condition...&-^;");
                return null;

            } else {

                Match match = regex.Match(text);
                if (match.Success) {

                    string numberPart = match.Groups[1].Value;

                    if (double.TryParse(numberPart, out double value)) {
                        PropertyItem property = new PropertyItem(value);
                        if (isLeft) propLeft = property; else propRight = property;
                        return property;
                    }
                } else {
                    PropertyItem property = new PropertyItem(text);
                    if (isLeft) propLeft = property; else propRight = property;
                    return property;
                }
                ignored = true;
                console.WriteLine($"  &-c;Constant value isn't numeric '&-e;{text}&-^;', ignoring condition...&-^;");
                return null;
            }
        }

    }

}