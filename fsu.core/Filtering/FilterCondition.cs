namespace Maxstupo.Fsu.Core.Filtering {

    using System;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    public class FilterCondition : IFilterEntry {

        public Operand LeftOperand { get; }

        public Operator Operator { get; }

        public Operand RightOperand { get; }

        public FilterCondition(Operand leftOperand, Operator op, Operand rightOperand) {
            LeftOperand = leftOperand ?? throw new ArgumentNullException(nameof(leftOperand));
            Operator = op;
            RightOperand = rightOperand ?? throw new ArgumentNullException(nameof(rightOperand));
        }

        public bool Evaluate(IOutput output, IPropertyProvider propertyProvider, IPropertyStore propertyStore, ProcessorItem item) {

            PropertyItem leftValue = LeftOperand.GetValue(output, propertyProvider, propertyStore, item);
            if (leftValue == null)
                return Operator.HasFlag(Operator.Ignore);


            PropertyItem rightValue = RightOperand.GetValue(output, propertyProvider, propertyStore, item);
            if (rightValue == null)
                return Operator.HasFlag(Operator.Ignore);


            if (leftValue.IsNumeric && rightValue.IsNumeric) { // If we are comparing two numbers.

                double vl = leftValue.ValueNumber;
                double vr = rightValue.ValueNumber;

                return EvaluateNumeric(vl, vr, output);

            } else if (!leftValue.IsNumeric && !rightValue.IsNumeric) { // If we are comparing two strings.              

                string vl = leftValue.ValueText;
                string vr = rightValue.ValueText;

                return EvaluateText(vl, vr, output);

            } else {
                output.WriteLine(Level.Warn, $"&-c;Condition type mismatch, unable to compare values, ignoring condition: {LeftOperand} {Operator} {RightOperand}&-^;");
                return true;
            }
        }

        private bool EvaluateNumeric(double vl, double vr, IOutput output) {

            if (Operator.HasFlag(Operator.StartsWith) || Operator.HasFlag(Operator.EndsWith) || Operator.HasFlag(Operator.Contains) || Operator.HasFlag(Operator.Regex)) {
                output.WriteLine(Level.Warn, $"&-c;Unsupported operator for numeric comparison &-e;'{Operator}'&-^;, unable to compare values, ignoring condition...&-^;");
                return true;
            }

            bool result = false;

            if (Operator.HasFlag(Operator.LessThan)) {
                result = vl < vr;
            } else if (Operator.HasFlag(Operator.GreaterThan)) {
                result = vl > vr;
            }

            if (Operator.HasFlag(Operator.Equal))
                result |= vl == vr;

            return result == !Operator.HasFlag(Operator.Not);

        }

        private bool EvaluateText(string vl, string vr, IOutput output) {
            bool isInverted = Operator.HasFlag(Operator.Not);

            if (Operator.HasFlag(Operator.StartsWith)) {
                return vl.StartsWith(vr, StringComparison.InvariantCultureIgnoreCase) == !isInverted;

            } else if (Operator.HasFlag(Operator.EndsWith)) {
                return vl.EndsWith(vr, StringComparison.InvariantCultureIgnoreCase) == !isInverted;

            } else if (Operator.HasFlag(Operator.Equal)) {
                return vl.Equals(vr, StringComparison.InvariantCultureIgnoreCase) == !isInverted;

            } else if (Operator.HasFlag(Operator.Contains)) {
                return vl.Contains(vr, StringComparison.InvariantCultureIgnoreCase) == !isInverted;

            }

            output.WriteLine(Level.Warn, $"&-c;Unsupported operator for string comparison &-e;'{Operator}'&-^;, unable to compare values, ignoring condition...");
            return true;
        }

    }

}