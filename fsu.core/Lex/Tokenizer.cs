using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maxstupo.Fsu.Core.Lex {

    public class Tokenizer {

        public string CommentKeyword { get; set; } = "//";

        //  public string NumberRegex { get; set; } = @"-?\d+(\.\d+)?";

        public Dictionary<char, TokenType> Symbols { get; } = new Dictionary<char, TokenType>();

        public Dictionary<string, TokenType> Keywords { get; } = new Dictionary<string, TokenType>();


        private readonly List<Token> tokens = new List<Token>();

        public Tokenizer() {
            CommentKeyword = "//";

            Symbols.Clear();
            Symbols.Add('=', TokenType.Operator);
            Symbols.Add('~', TokenType.Operator);
            Symbols.Add('!', TokenType.Operator);
            Symbols.Add('<', TokenType.Operator);
            Symbols.Add('>', TokenType.Operator);
            Symbols.Add('&', TokenType.Operator);
            Symbols.Add('|', TokenType.Operator);
            Symbols.Add(',', TokenType.Delim);

            Keywords.Clear();
            Keywords.Add(">>", TokenType.Pipe);
        }

        public TokenList Parse(params string[] lines) {
            tokens.Clear();

            for (int i = 0; i < lines.Length; i++)
                ParseLine(lines[i], i);

            return new TokenList(tokens);
        }

        private void ParseLine(string line, int lineNumber) {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith(CommentKeyword))
                return;

            for (int letterIndex = 0; letterIndex < line.Length; letterIndex++) {
                char c = line[letterIndex];
                if (c == '\t') c = ' ';

                if (Symbols.TryGetValue(c, out TokenType type)) {

                    TokenLocation tokenLocation = new TokenLocation(lineNumber, letterIndex);

                    if (letterIndex + 1 < line.Length && c == line[letterIndex + 1] && Keywords.TryGetValue(new string(c, 2), out TokenType spec)) {
                        tokens.Add(new Token(spec, new string(c, 2), 0, tokenLocation));
                        letterIndex += 1;

                    } else {
                        tokens.Add(new Token(type, char.ToString(c), 0, tokenLocation));

                    }

                } else if (!char.IsWhiteSpace(c)) {
                    Token token = GetText(line, lineNumber, letterIndex);
                    letterIndex += token.Length - 1;

                    tokens.Add(token);

                }

            }

        }

        private Token GetText(string input, int lineNumber, int letterIndex) {
            StringBuilder sb = new StringBuilder();
            bool inQuote = false;
            bool isQuoted = false;
            bool hasMatchingQuote = false;

            for (int j = letterIndex; j < input.Length; j++) {
                char c = input[j];

                if (!inQuote && (Symbols.ContainsKey(c) || char.IsWhiteSpace(c)))
                    break;


                if (c == '"') {
                    if (j != letterIndex)
                        break;

                    if (!inQuote) {
                        hasMatchingQuote = false;
                        for (int i = j + 1; i < input.Length; i++) {
                            if (input[i] == '"') {
                                hasMatchingQuote = true;
                                break;
                            }
                        }
                    }

                    if (hasMatchingQuote) {
                        isQuoted = true;
                        inQuote = !inQuote;
                        if (!inQuote)
                            break;
                    } else {
                        throw new Exception("Mismatched double quote @line #" + lineNumber);
                    }
                } else {
                    sb.Append(c);
                }
            }

            string output = sb.ToString();

            if (!Keywords.TryGetValue(output, out TokenType type))
                type = TokenType.Text;

            int lengthOffset = 0;
            if (isQuoted) {
                type = TokenType.QuotedText;
                lengthOffset += 2;

            } else if (output.All(x => char.IsDigit(x) || x == '-' || x == '.')) { // TODO: Replace with regex
                type = TokenType.Number;
            }

            return new Token(type, output, lengthOffset, new TokenLocation(lineNumber, letterIndex));
        }

    }

}
