namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents something that can convert text into a series of tokens.
    /// </summary>
    public interface ITokenizer<T> where T : Enum {

        /// <summary>
        /// Registers the specified <see cref="TokenDefinition{T}"/> with this tokenizer.
        /// </summary>
        void Add(TokenDefinition<T> definition);

        void Remove(TokenDefinition<T> definition);
        
        /// <summary>
        /// Clears all registered token definitions.
        /// </summary>
        void Clear();

        /// <summary>
        /// Tokenizes an enumerable, treating each item in the enumerable as a seperate line.
        /// </summary>
        /// <returns>The tokenized representation of the provided input.</returns>
        IEnumerable<Token<T>> Tokenize(IEnumerable<string> input);

        /// <summary>
        /// Tokenizes a given string, treating it as a single line of text.
        /// </summary>
        /// <returns>The tokenized representation of the provided input.</returns>
        IEnumerable<Token<T>> Tokenize(string input, int lineNumber);

        /// <summary>
        /// Dynamically loads token definitions using reflection. Finds <see cref="TokenDef"/> attributes.
        /// </summary>
        void LoadTokenDefinitions();

    }

}