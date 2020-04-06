namespace Maxstupo.Fsu.Core.Utility {

    public interface IConsole {

        void Write(char c);

        void Write(string str, bool disableColor = false);

        void WriteLine();

        void WriteLine(string line, bool disableColor = false);

        void Clear();

    }

}