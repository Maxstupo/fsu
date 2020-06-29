namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A list of tokens, with features simialr to a <see cref="Stack"/>.
    /// </summary>
    public class TokenStack<T> : IEnumerable<Token<T>> where T : Enum {

        /// <summary>Returns true if calling <see cref="Next"/> will result in a next element.</summary>
        public bool HasNext => index < list.Count - 1;

        /// <summary>Returns true if calling <see cref="Prev"/> will result in a previous element.</summary>
        public bool HasPrev => index >= 0;


        private int index;

        private readonly List<Token<T>> list;

        private readonly Stack<int> recordStack = new Stack<int>();

        public TokenStack() : this(Enumerable.Empty<Token<T>>()) { }

        public TokenStack(IEnumerable<Token<T>> collection) {
            list = new List<Token<T>>(collection);
            Reset();
        }

        /// <summary>
        /// Records the current index in a stack, so we can jump to it in the future.
        /// </summary>
        public void Mark() {
            recordStack.Push(index);
        }

        /// <summary>
        /// Jumps to the next recorded index mark made by <see cref="Mark"/>. If no marks are recorded this method does nothing.
        /// </summary>
        public void Jump() {
            if (recordStack.Count > 0)
                index = recordStack.Pop();
        }

        /// <summary>
        /// Sets the index of this stack to the beggining and clears all marks.
        /// </summary>
        /// <returns></returns>
        public TokenStack<T> Reset() {
            index = -1;
            recordStack.Clear();
            return this;
        }

        /// <summary>
        /// Returns the next item. If the end of the list is reached, the last element will always be returned until <see cref="Reset"/> is called.
        /// </summary>
        public Token<T> Next() {
            if (HasNext)
                index++;
            return list[index];
        }

        /// <summary>
        /// Returns the previous item. If the start of the list is reached, the first element will always be returned.
        /// </summary>
        public Token<T> Prev() {
            if (HasPrev) {
                index--;

                return list[Math.Max(index, 0)];
            }
            return list[index];
        }

        /// <summary>
        /// Returns the current item, without advancing to the next element.
        /// </summary>
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