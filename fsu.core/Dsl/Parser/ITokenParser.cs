namespace Maxstupo.Fsu.Core.Dsl.Parser {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using System;
    using System.Collections.Generic;

    public interface ITokenParser<T, V> where T : Enum where V : class {
        
        event EventHandler<Grammar<T, V>> OnGrammerAdded;
        event EventHandler<Grammar<T, V>> OnGrammerRemoved;
        event EventHandler OnGrammersCleared;
        event EventHandler<Token<T>> OnTokenError;
        event EventHandler<Token<T>> OnTokenParsing;

        Grammar<T, V> Add(Grammar<T, V> grammer);

        void Remove(Grammar<T, V> grammer);

        void Clear();

        List<V> Parse(IEnumerable<Token<T>> tokens);

    }

}