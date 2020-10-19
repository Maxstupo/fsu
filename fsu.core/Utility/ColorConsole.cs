namespace Maxstupo.Fsu.Core.Utility {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    public class ColorConsole : IOutput {
        private static readonly Regex Regex = new Regex(@"\&([0-9a-fA-F\^\-]{1})([0-9a-fA-F\^\-]{1})\;", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private readonly object _lock = new object();

        private readonly Stack<ConsoleColor> foregroundStack = new Stack<ConsoleColor>();
        private readonly Stack<ConsoleColor> backgroundStack = new Stack<ConsoleColor>();

        private readonly TextWriter output;

        public Level Level { get; set; } = Level.Fine;

        public ColorConsole(TextWriter output = null) {
            this.output = output ?? Console.Out;

            Push(ConsoleColor.Black, ConsoleColor.White);
        }

        public void Clear() {
            lock (_lock)
                Console.Clear();
        }

        public void WriteLine(Level level) {
            if (!CanWrite(level))
                return;

            lock (_lock)
                output.WriteLine();
        }

        /// <summary>
        /// Accepts color stack syntax. 
        /// </summary>
        public void WriteLine(Level level, string line, bool disableColor = false) {
            if (!CanWrite(level))
                return;

            if (!disableColor) {
                WriteText(line, true);
            } else {
                lock (_lock)
                    output.WriteLine(line);
            }
        }

        /// <summary>
        /// Accepts color stack syntax. 
        /// </summary>
        public void Write(Level level, string str, bool disableColor = false) {
            if (!CanWrite(level))
                return;

            if (!disableColor) {
                WriteText(str, false);
            } else {
                lock (_lock)
                    output.Write(str);
            }
        }


        public void Write(Level level, char c) {
            if (!CanWrite(level))
                return;
            lock (_lock)
                output.Write(c);
        }


        private void WriteText(string text, bool newline) {
            lock (_lock) {
                text += "&--;";

                int index = 0;
                MatchCollection mc = Regex.Matches(text);
                foreach (Match match in mc) {
                    if (!match.Success)
                        continue;

                    string substring = text.Substring(index, match.Index - index);
                    index = match.Index + match.Length;

                    output.Write(substring);

                    string bgStr = match.Groups[1].Value;
                    string fgStr = match.Groups[2].Value;

                    if (bgStr.Equals("^", StringComparison.InvariantCultureIgnoreCase)) {
                        PopBg();
                    } else if (int.TryParse(bgStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int bgColor) && Enum.IsDefined(typeof(ConsoleColor), bgColor)) {
                        PushBg((ConsoleColor) bgColor);
                    }

                    if (fgStr.Equals("^", StringComparison.InvariantCultureIgnoreCase)) {
                        PopFg();
                    } else if (int.TryParse(fgStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int fgColor) && Enum.IsDefined(typeof(ConsoleColor), fgColor)) {
                        PushFg((ConsoleColor) fgColor);
                    }
                }

                if (newline)
                    output.Write(Environment.NewLine);
            }
        }

        private bool CanWrite(Level level) {
            return level == Level.None || level >= this.Level;
        }

        /// <summary>
        /// Sets the current console foreground color and adds it to the foreground stack.
        /// </summary>
        public void PushForeground(ConsoleColor fg) {
            lock (_lock)
                PushFg(fg);
        }

        /// <summary>
        /// Sets the current console background color and adds it to the background stack.
        /// </summary>
        public void PushBackground(ConsoleColor bg) {
            lock (_lock)
                PushBg(bg);
        }

        /// <summary>
        /// Pops the foreground stack and sets the current foreground console color.
        /// </summary>
        public void PopForeground() {
            lock (_lock)
                PopFg();
        }

        /// <summary>
        /// Pops the background stack and sets the current background console color.
        /// </summary>
        public void PopBackground() {
            lock (_lock)
                PopBg();
        }

        public void Push(ConsoleColor bg, ConsoleColor fg) {
            lock (_lock) {
                PushBg(bg);
                PushFg(fg);
            }
        }

        public void Pop() {
            lock (_lock) {
                PopBg();
                PopFg();
            }
        }

        private void PushFg(ConsoleColor fg) {
            foregroundStack.Push(Console.ForegroundColor);
            Console.ForegroundColor = fg;
        }

        private void PushBg(ConsoleColor bg) {
            backgroundStack.Push(Console.BackgroundColor);
            Console.BackgroundColor = bg;
        }

        private void PopFg() {
            if (foregroundStack.Count > 0)
                Console.ForegroundColor = foregroundStack.Pop();
        }

        private void PopBg() {
            if (backgroundStack.Count > 0)
                Console.BackgroundColor = backgroundStack.Pop();
        }

    }

}