using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Lex {

    public class TokenList : IEnumerable<Token> {
        private readonly List<Token> tokens = new List<Token>();

        public int Index { get; private set; }

        public int Count => tokens.Count;

        public bool HasNext => Index < (Count - 1);


        public TokenList(IEnumerable<Token> tokens) {
            if (tokens != null)
                this.tokens.AddRange(tokens);
            Reset();
        }

        public TokenList Reset() {
            Index = -1;
            return this;
        }

        public Token Next() {
            if (HasNext)
                Index++;
            return Peek();
        }

        public Token Prev() {
            if (Index > 0)
                Index--;
            return Peek();
        }

        public Token Peek() {
            return tokens[Index];
        }

        public void Print() {
            int padding = 0;
            foreach (Token token in this) {
                Console.WriteLine($"{new string(' ', padding)}{token.Value,-20}{new string(' ', 20 - padding)}{token.Type}");
                if (token.Type == TokenType.Pipe)
                    padding += 4;
            }
        }

        public TokenList Copy() {
            return new TokenList(tokens);
        }

        public IEnumerator<Token> GetEnumerator() {
            return tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

}
