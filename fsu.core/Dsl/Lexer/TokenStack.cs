namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class TokenStack<T> : IEnumerable<Token<T>> where T : Enum {

        public bool HasNext => index < list.Count - 1;
        public bool HasPrev => index >= 0;


        private int index;

        private readonly List<Token<T>> list;

        private readonly Stack<int> recordStack = new Stack<int>();

        public TokenStack() : this(Enumerable.Empty<Token<T>>()) { }

        public TokenStack(IEnumerable<Token<T>> collection) {
            list = new List<Token<T>>(collection);
            Reset();
        }


        public void Mark() {
            recordStack.Push(index);
        }

        public void Jump() {
            if (recordStack.Count > 0)
                index = recordStack.Pop();
        }

        public TokenStack<T> Reset() {
            index = -1;
            recordStack.Clear();
            return this;
        }

        public Token<T> Next() {
            if (HasNext)
                index++;
            return list[index];
        }

        public Token<T> Prev() {
            if (HasPrev) {
                index--;

                return list[Math.Max(index, 0)];
            }
            return list[index];
        }

        public Token<T> Peek() {
            return list[index];
        }

        public IEnumerator<Token<T>> GetEnumerator() {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
 
    }

}