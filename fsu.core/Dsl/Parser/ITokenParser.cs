namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Dsl.Lexer;

    public interface ITokenParser<T, V> where T : Enum where V : class {
        
        event EventHandler<Grammar<T, V>> OnGrammarAdded;
        event EventHandler<Grammar<T, V>> OnGrammarRemoved;
        event EventHandler OnGrammarsCleared;

        event EventHandler<Token<T>> OnTokenError;
        event EventHandler<Token<T>> OnTokenParsing;

        Grammar<T, V> Add(Grammar<T, V> grammar);

        void Remove(Grammar<T, V> grammar);

        void Clear();

        List<V> Parse(IEnumerable<Token<T>> tokens);

    }

}