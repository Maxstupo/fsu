using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Format {
    public class FormatTemplate {
        private static readonly Regex regex = new Regex(@"([\@\$])\{(\w+)(?:\:([\d\w]{1,2})(?:\,([\d\w]{1,2}))?)?\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly List<FormatToken> tokens;

        private FormatTemplate(List<FormatToken> tokens) {
            this.tokens = tokens;
        }

        public void Print() {
            foreach (var item in tokens)
                Console.WriteLine($"{$"'{item.Value}'",-20}{item.Type,-9}{item.Decimals,-4}{item.Unit,-6}(n / {(double) item.Unit})");
        }

        public string Make(IFilePropertyProvider propertyProvider, IPropertyStore propertyContainer, ProcessorItem item) {
            StringBuilder sb = new StringBuilder();
            propertyProvider.Begin();
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


                bool isFileProperty = match.Groups[1].Value == "@";
                string name = match.Groups[2].Value;

                int decimals = 2;
                FormatUnit unit = FormatUnit.None;

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

                PropertyType type = isFileProperty ? PropertyType.File : PropertyType.Global;
                tokens.Add(new FormatToken(name, type, decimals, unit));

                endIndex = match.Index + match.Length;
                if (j == mc.Count - 1 && endIndex < format.Length) // Add trailing text 
                    tokens.Add(new FormatToken(format.Substring(endIndex), PropertyType.Text));
            }

            if (mc.Count == 0)
                tokens.Add(new FormatToken(format, PropertyType.Text));

            return Build(tokens);
        }

        public bool ContainsFileProperty(params string[] items) {
            return tokens.Where(x => x.Type == PropertyType.File).Any(x => x.Value.ContainsAny(items, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool ContainsText(params string[] items) {
            return tokens.Where(x => x.IsText).Any(x => x.Value.ContainsAny(items, StringComparison.InvariantCultureIgnoreCase));
        }

        public static FormatTemplate Build(List<FormatToken> tokens) {
            return new FormatTemplate(tokens);
        }

        public static FormatTemplate Build(params FormatToken[] tokens) {
            return Build(tokens.ToList());
        }
    }

}
