using Maxstupo.Fsu.Core.Lex;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maxstupo.Fsu.Core {
    public class TokenParser {

        private static string GetText(TokenList list) {
            StringBuilder sb = new StringBuilder();

            while (list.HasNext) {

                sb.Append(list.Peek().Value + " ");

                if (list.Next().Type != TokenType.Text) {
                    list.Prev();
                    break;
                }
            }

            if (sb.Length > 0) 
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static bool Parse(ProcessorPipeline pipe, TokenList list, out List<string> startingItems) {
            pipe.Clear();

            if (list == null) {
                startingItems = null;
                return false;
            }

            list.Reset();

            bool expectingDelim = false;
            int pipeCount = 0;

            startingItems = new List<string>();

            while (list.HasNext) {
                Token t = list.Next();

                if (expectingDelim && t.Type != TokenType.Delim && t.Type != TokenType.Pipe) {
                    ColorConsole.WriteLine($"&0c;Expected delim {t.Location}&^^;");
                    return false;
                }

                if (pipeCount != 0 && t.Type != TokenType.Function) {
                    ColorConsole.WriteLine($"&0c;Expected function{t.Location}&^^;");
                    return false;
                }

                switch (t.Type) {
                    case TokenType.Pipe:
                        expectingDelim = false;
                        pipeCount++;
                        break;

                    case TokenType.Text:
                        string text = GetText(list);
                        startingItems.Add(text.Replace('/', '\\'));
                        expectingDelim = true;
                        break;

                    case TokenType.QuotedText:
                        startingItems.Add(t.Value.Replace('/', '\\'));
                        expectingDelim = true;
                        break;

                    case TokenType.Delim:
                        expectingDelim = false;
                        break;

                    case TokenType.Function:
                        expectingDelim = false;
                        if (!ParseFunction(pipe, list))
                            return false;

                        break;

                    case TokenType.Operator:
                    case TokenType.Number:
                        ColorConsole.WriteLine($"&0c;Operator / Number unexpected {t.Location}&^^;");
                        return false;

                    default:
                        throw new NotSupportedException();
                }



            }
            return true;
        }

        private static bool ParseFunction(ProcessorPipeline pipe, TokenList list) {
            string name = list.Peek().Value;
            //  ColorConsole.WriteLine($"&0e;######### Function ({name}) #########&^^;");

            List<Token> tokens = new List<Token>();

            bool done = false;
            while (list.HasNext && !done) {
                Token t = list.Next();

                switch (t.Type) {
                    case TokenType.Text:
                    case TokenType.QuotedText:
                    case TokenType.Number:
                    case TokenType.Operator:
                    case TokenType.Delim:
                        tokens.Add(t);
                        //  ColorConsole.WriteLine($"&02;{t.Value,-15}{t.Type}&^^;");
                        break;
                    case TokenType.Pipe:
                        done = true;
                        break;
                    case TokenType.Function:
                        //   ColorConsole.WriteLine($"&0c;Unexpected function {t.Location}&^^;");
                        return false;
                    default:
                        throw new NotSupportedException();
                }

            }

            if (ProcessorManager.Instance.Construct(name, new TokenList(tokens), out IProcessor processor, out string error)) {
                pipe.Add(processor);
            } else {
                ColorConsole.WriteLine($"&0c;Function '{name}' {error}&^^;");
                return false;
            }

            //   Console.WriteLine();
            return true;
        }
    }

}
