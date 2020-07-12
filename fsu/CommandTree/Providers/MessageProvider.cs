namespace Maxstupo.Fsu.CommandTree.Providers {

    public enum StandardMessages {
        NoCommand = 0,
        DetailedHelpTip = 1,
        NextHelpPageTip = 2,
        CurrentHelpPage = 3,
        HelpTitle = 4,
        TitleDivider = 5,
        DetailedHelpParam = 6,
        DetailedHelpName = 7,
        DetailedHelpDescription = 8,
        DetailedHelpKeyword = 9,
        DetailedHelpTitle = 10,
        DetailedHelpAliases = 11,
        DetailedHelpUsage = 12,
        DetailedHelpParamTitle = 13,
        DetailedHelpSubCommandTitle = 14,
        DetailedHelpSubCommand = 15
    }

    public abstract class MessageProvider {

        public abstract void Set(string key, string messageTemplate);

        public abstract string Get(string key, string defaultMessage = "");

        public abstract string GetFormatted(string key, params object[] args);

        protected virtual string GetKey(StandardMessages standardMessage) {
            return $"std_{standardMessage.ToString().ToLower()}";
        }

        public void Set(StandardMessages key, string messageTemplate) {
            Set(GetKey(key), messageTemplate);
        }

        public string Get(StandardMessages key, string defaultMessage = "") {
            return Get(GetKey(key), defaultMessage);
        }

        public string GetFormatted(StandardMessages key, params object[] args) {
            return GetFormatted(GetKey(key), args);
        }

    }

}