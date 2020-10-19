namespace Maxstupo.Fsu.Core.Dsl.Lexer {
 
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITokenizer<T> where T : Enum {

        event EventHandler<TokenDefinition<T>> OnDefinitionAdded;
        event EventHandler<TokenDefinition<T>> OnDefinitionRemoved;
        event EventHandler OnDefinitionsCleared;

        /// <summary>
        /// Registers the specified <paramref name="tokenDefinition"/> with this tokenizer.
        /// </summary>
        void Add(TokenDefinition<T> tokenDefinition);

        /// <summary>
        /// Removes the specified <paramref name="tokenDefinition"/> from this tokenizer. Does nothing if the token doesn't exist.
        /// </summary>
        void Remove(TokenDefinition<T> tokenDefinition);

        /// <summary>
        /// Clears all registered token definitions.
        /// </summary>
        void Clear();

        /// <summary>
        /// Tokenizes an enumerable. Each item in the enumerable is considered a seperate line.
        /// </summary>
        /// <returns>The tokenized representation of the provided input.</returns>
        IEnumerable<Token<T>> Tokenize(IEnumerable<string> input);

        /// <summary>
        /// Tokenizes a given string.
        /// </summary>
        /// <returns>The tokenized representation of the provided input.</returns>
        IEnumerable<Token<T>> Tokenize(string input, int lineNumber);

        /// <summary>
        /// Dynamically loads token definitions using reflection. Finds <see cref="TokenDef"/> attributes.
        /// </summary>
        void LoadTokenDefinitions();

    }

}