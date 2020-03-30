using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Maxstupo.Fsu.Core.Utility {

    public static class ColorConsole {

        private static readonly object _lock = new object();

        private static readonly Stack<ConsoleColor> foregroundStack = new Stack<ConsoleColor>();
        private static readonly Stack<ConsoleColor> backgroundStack = new Stack<ConsoleColor>();

        private static readonly Regex regex = new Regex(@"\&([0-9a-fA-F\^\-]{1})([0-9a-fA-F\^\-]{1})\;", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);


        static ColorConsole() {
            Push(ConsoleColor.Black, ConsoleColor.White);
        }

        /// <inheritdoc cref="Console.WriteLine()"/>  
        public static void WriteLine() {
            lock (_lock)
                Console.WriteLine();
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string)"/>.<br/><br/>
        /// Accepts color stack syntax. 
        /// </summary>
        /// <inheritdoc cref="Console.WriteLine(string)"/>
        public static void WriteLine(string line) {
            WriteText(line, true);
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string)"/>.<br/><br/>
        /// Accepts color stack syntax. 
        /// </summary>
        /// <inheritdoc cref="Console.Write(string)"/>
        public static void Write(string str) {
            WriteText(str, false);
        }

        /// <inheritdoc cref="Console.Write(char)"/>
        public static void Write(char c) {
            lock (_lock)
                Console.Write(c);
        }

        private static void WriteText(string text, bool newline) {
            lock (_lock) {
                text += "&--;";

                int index = 0;
                MatchCollection mc = regex.Matches(text);
                foreach (Match match in mc) {
                    if (!match.Success)
                        continue;

                    string substring = text.Substring(index, match.Index - index);
                    index = match.Index + match.Length;

                    Console.Write(substring);

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
                    Console.Write(Environment.NewLine);
            }
        }

        /// <summary>
        /// Sets the current console foreground color and adds it to the foreground stack.
        /// </summary>
        public static void PushForeground(ConsoleColor fg) {
            lock (_lock)
                PushFg(fg);
        }

        /// <summary>
        /// Sets the current console background color and adds it to the background stack.
        /// </summary>
        public static void PushBackground(ConsoleColor bg) {
            lock (_lock)
                PushBg(bg);
        }

        /// <summary>
        /// Pops the foreground stack and sets the current foreground console color.
        /// </summary>
        public static void PopForeground() {
            lock (_lock)
                PopFg();
        }

        /// <summary>
        /// Pops the background stack and sets the current background console color.
        /// </summary>
        public static void PopBackground() {
            lock (_lock)
                PopBg();
        }

        public static void Push(ConsoleColor bg, ConsoleColor fg) {
            lock (_lock) {
                PushBg(bg);
                PushFg(fg);
            }
        }

        public static void Pop() {
            lock (_lock) {
                PopBg();
                PopFg();
            }
        }

        private static void PushFg(ConsoleColor fg) {
            foregroundStack.Push(Console.ForegroundColor);
            Console.ForegroundColor = fg;
        }

        private static void PushBg(ConsoleColor bg) {
            backgroundStack.Push(Console.BackgroundColor);
            Console.BackgroundColor = bg;
        }

        private static void PopFg() {
            if (foregroundStack.Count > 0)
                Console.ForegroundColor = foregroundStack.Pop();
        }

        private static void PopBg() {
            if (backgroundStack.Count > 0)
                Console.BackgroundColor = backgroundStack.Pop();
        }

    }

}
