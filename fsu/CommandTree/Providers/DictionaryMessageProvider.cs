namespace Maxstupo.Fsu.CommandTree.Providers {

    using System.Collections.Generic;
    using System.Text;

    public sealed class DictionaryMessageProvider : MessageProvider {

        private readonly Dictionary<string, string> messageTemplates = new Dictionary<string, string>();

        public DictionaryMessageProvider() {
            Set(StandardMessages.NoCommand, "No command found for '{0}'");

            Set(StandardMessages.TitleDivider, "=");
            Set(StandardMessages.HelpTitle, "{0} [ Help ] {0}");
            Set(StandardMessages.NextHelpPageTip, "Type 'help {0}( {1})' to read the next page.");
            Set(StandardMessages.CurrentHelpPage, "\nPage: {0} / {1}");
            Set(StandardMessages.DetailedHelpTip, "\nFor detailed help append '?' to the end of any command.");

            Set(StandardMessages.DetailedHelpTitle, "--------------- [ Command Help ] ---------------");
            Set(StandardMessages.DetailedHelpName, "Name: {0}");
            Set(StandardMessages.DetailedHelpDescription, "(Description: {0})");
            Set(StandardMessages.DetailedHelpKeyword, "Keyword: {0}");
            Set(StandardMessages.DetailedHelpAliases, "(Aliases: {0})");
            Set(StandardMessages.DetailedHelpUsage, "Usage: {0}");
            Set(StandardMessages.DetailedHelpParamTitle, "\nParameters:");
            Set(StandardMessages.DetailedHelpParam, "  {0}( \\({2}\\))( - {1})");

            Set(StandardMessages.DetailedHelpSubCommandTitle, "\nSub-Commands:");
            Set(StandardMessages.DetailedHelpSubCommand, "  - {0}");
        }


        public override string Get(string key, string defaultMessage = "") {
            return messageTemplates.TryGetValue(key, out string value) ? value : defaultMessage;
        }


        public override string GetFormatted(string key, params object[] args) {
            string template = Get(key);

            string resolvedTemplate = ResolveStringTemplate(template, args);

            return string.Format(resolvedTemplate, args);
        }


        public override void Set(string key, string messageTemplate) {
            if (messageTemplates.ContainsKey(key))
                messageTemplates.Remove(key);
            messageTemplates.Add(key, messageTemplate);
        }

        public static string ResolveStringTemplate(string template, params object[] args) {

            bool escapeNext = false;
            int start = -1;
            int end = -1;
            bool inBlock = false;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < template.Length; i++) {
                char c = template[i];


                if (c == '\\' && !escapeNext) {
                    escapeNext = true;
                    continue;
                } else if (!escapeNext) {
                    if (c == '(' && !inBlock) {
                        start = i;
                        inBlock = true;
                    } else if (c == ')' && inBlock) {
                        end = i;
                        string templateBlock = ProcessTemplateBlock(template.Substring(start + 1, end - start - 1), args);
                        sb.Append(templateBlock);
                        inBlock = false;
                    } else if (!inBlock) {
                        sb.Append(c);
                    }
                } else if (escapeNext && !inBlock) {
                    sb.Append(c);
                }


                escapeNext = false;
            }

            return sb.ToString();
        }

        public static string ProcessTemplateBlock(string template, params object[] args) {
            template = template.Replace("\\", string.Empty);

            string prefix = string.Empty, suffix = string.Empty, not = string.Empty;
            string value = null;
            int start = -1, startNot = -1;

            for (int i = 0; i < template.Length; i++) {
                char c = template[i];

                if (c == '{') {
                    prefix = template.Substring(0, i);
                    start = i;
                } else if (c == '}') {
                    value = template.Substring(start, i - start + 1);
                    start = i + 1;
                } else if (c == '|') {
                    startNot = i + 1;
                    suffix = template.Substring(start, i - start);
                }
            }

            if (startNot != -1)
                not = template.Substring(startNot, template.Length - startNot);
            else
                suffix = template.Substring(start, template.Length - start);

            string textValue = string.Format(value, args);

            if (string.IsNullOrEmpty(textValue)) {
                return not;
            } else {

                return string.Format("{0}{1}{2}", prefix, textValue, suffix);
            }
        }

    }

}