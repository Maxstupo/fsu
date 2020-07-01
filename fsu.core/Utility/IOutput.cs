namespace Maxstupo.Fsu.Core.Utility {

    public enum Level : int {
        None = -1,
        Fine = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Severe = 5
    }

    public interface IOutput {

        Level Level { get; set; }

        void Write(Level level, char c);

        void Write(Level level, string str, bool disableColor = false);

        void WriteLine(Level level);

        void WriteLine(Level level, string line, bool disableColor = false);

        void Clear();

    }

}