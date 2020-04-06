﻿using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maxstupo.Fsu {
    public class Cli {
        public string Prompt { get; set; } = "&-e;>>&-^; ";
        public int MaxCommandHistory { get; set; } = 25;

        public event EventHandler<string> OnCommand;
        public event Func<string, int, string> OnAutoComplete; // TODO: Use a IAutoCompleteProvider pattern.

        private int caretIndex = 0;
        private readonly StringBuilder sb = new StringBuilder();

        private int historyIndex = 0;
        private readonly List<string> commandHistory = new List<string>();

        private readonly IConsole console;

        public Cli(IConsole console) {
            this.console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void Run() {

            while (true) {
                if (!string.IsNullOrWhiteSpace(Prompt))
                    console.Write(Prompt);

                while (true) {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    char keyChar = keyInfo.KeyChar;


                    if (keyInfo.Key == ConsoleKey.Enter)
                        break;


                    switch (keyInfo.Key) {
                        case ConsoleKey.Backspace:
                            Backspace();
                            break;
                        case ConsoleKey.UpArrow:
                            ShowHistory(-1);
                            break;
                        case ConsoleKey.DownArrow:
                            ShowHistory(1);
                            break;
                        case ConsoleKey.LeftArrow:
                            MoveCaretLeft();
                            break;
                        case ConsoleKey.RightArrow:
                            MoveCaretRight();
                            break;
                        case ConsoleKey.Tab:
                            AttemptAutocomplete();
                            break;
                        default:
                            if (char.IsWhiteSpace(keyChar) || char.IsPunctuation(keyChar) || char.IsSymbol(keyChar) || char.IsLetterOrDigit(keyChar))
                                InsertChar(keyChar);
                            break;
                    }


                }

                console.WriteLine();

                string cmd = sb.ToString().Trim();
                if(!string.IsNullOrWhiteSpace(cmd))
                commandHistory.Add(cmd);
                historyIndex = commandHistory.Count;

                if (commandHistory.Count > MaxCommandHistory)
                    commandHistory.RemoveAt(0);

                OnCommand?.Invoke(this, cmd);

                sb.Clear();
                caretIndex = 0;
            }
        }


        private void ShowHistory(int dir) {
            if (commandHistory.Count == 0)
                return;

            historyIndex += Math.Sign(dir);
            if (historyIndex < 0)
                historyIndex = commandHistory.Count - 1;
            if (historyIndex >= commandHistory.Count)
                historyIndex = 0;

            Clear();
            InsertString(commandHistory[historyIndex]);
        }

        private void AttemptAutocomplete() {
            string output = OnAutoComplete?.Invoke(sb.ToString(), caretIndex) ?? null;
            if (!string.IsNullOrEmpty(output)) {
                while (MoveCaretRight()) ;
                InsertString(output);
            }
        }

        private void Clear() {
            while (MoveCaretRight()) ;
            while (Backspace()) ;
        }


        private void InsertString(string text) {
            for (int i = 0; i < text.Length; i++)
                InsertChar(text[i]);
        }

        private void InsertChar(char keyChar) {
            sb.Insert(caretIndex, keyChar); // Insert the new char into the buffer.
            console.Write(keyChar);    // Write the new char to console.

            caretIndex++;

            for (int i = caretIndex; i < sb.Length; i++) // Replace the previous chars from buffer.
                console.Write(sb[i]);

            console.Write(new string('\b', sb.Length - caretIndex), true);//Return to original caret location
        }


        private bool MoveCaretRight() {
            if (caretIndex < sb.Length && sb.Length > 0) {
                console.Write(sb[caretIndex]);
                caretIndex++;
                return true;
            }
            return false;
        }

        private bool MoveCaretLeft() {
            if (caretIndex > 0 && sb.Length > 0) {
                console.Write('\b');
                caretIndex--;
                return true;
            }
            return false;
        }

        private bool Backspace() {
            if (sb.Length > 0) {

                console.Write('\b');

                for (int i = caretIndex; i < sb.Length; i++) // Replace the previous chars from buffer.
                    console.Write(sb[i]);

                console.Write(' ');

                caretIndex--;
                sb.Remove(caretIndex, 1);


                console.Write(new string('\b', (sb.Length - caretIndex) + 1), true);

                return true;
            }
            return false;
        }
    }
}
