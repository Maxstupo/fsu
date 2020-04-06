using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Format {

    public class FormatTemplate {
        private static readonly Regex regex = new Regex(@"([\@\$])\{(\w+)(?:\:([\d\w]{1,2})(?:\,([\d\w]{1,2}))?)?\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly List<FormatToken> tokens;

        public static readonly FormatTemplate Empty = Build(string.Empty);

        public string Template { get; }

        private FormatTemplate(string template, List<FormatToken> tokens) {
            this.Template = template;
            this.tokens = tokens;

        }

        /// <summary>
        /// Creates a string formatted using this template, with the values provided by the property providers.
        /// </summary>
        public string Make(IPropertyProvider propertyProvider, IPropertyStore propertyContainer, ProcessorItem item) {
            StringBuilder sb = new StringBuilder();

            propertyProvider.Begin();
            {

                foreach (FormatToken token in tokens) {
                    if (token.IsText) {
                        sb.Append(token.Value);

                    } else {
                        Property property = token.GetProperty(propertyProvider, propertyContainer, item);

                        if (property == null) {
                            sb.Append("NA");

                        } else if (property.IsNumeric) {
                            double value = Math.Round(property.ValueNumber / (double) token.Unit, token.Decimals);
                            sb.Append(value);

                        } else {
                            sb.Append(property.Value);

                        }
                    }
                }

            }
            propertyProvider.End();

            return sb.ToString();
        }

        public static FormatTemplate Build(string format) {
            List<FormatToken> tokens = new List<FormatToken>();

            int endIndex = -1;
            MatchCollection mc = regex.Matches(format);
            for (int j = 0; j < mc.Count; j++) {
                Match match = mc[j];
                if (!match.Success || !match.Groups[1].Success || !match.Groups[2].Success)
                    continue;

                if (j == 0 && match.Index > 0) // Add leading text.
                    tokens.Add(new FormatToken(format.Substring(0, match.Index), PropertyType.Text));

                int len = match.Index - endIndex;
                if (j > 0 && len > 0) // Add text between each match.
                    tokens.Add(new FormatToken(format.Substring(endIndex, len), PropertyType.Text));


                bool isItemProperty = match.Groups[1].Value == "@";
                string name = match.Groups[2].Value;

                int decimals = 2;
                Unit unit = Unit.None;

                for (int i = 3; i <= 4; i++) {// Group 3 and 4 could be decimal number or unit.
                    if (!match.Groups[i].Success)
                        continue;

                    string g3 = match.Groups[i].Value;

                    if (g3.All(x => char.IsDigit(x))) {
                        int.TryParse(g3, out decimals);

                    } else if (g3.All(x => char.IsLetter(x))) {
                        Enum.TryParse(g3, true, out unit);

                    }
                }

                PropertyType type = isItemProperty ? PropertyType.Item : PropertyType.Global;
                tokens.Add(new FormatToken(name, type, decimals, unit));

                endIndex = match.Index + match.Length;
                if (j == mc.Count - 1 && endIndex < format.Length) // Add trailing text 
                    tokens.Add(new FormatToken(format.Substring(endIndex), PropertyType.Text));
            }

            if (mc.Count == 0)
                tokens.Add(new FormatToken(format, PropertyType.Text));

            return new FormatTemplate(format, tokens);
        }


    }

}
