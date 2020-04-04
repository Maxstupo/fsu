using Maxstupo.Fsu.Core.Dsl.Lexer;
using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Dsl.Parser {
    public interface ITokenParser<T, V> where T : Enum where V : class {

        Grammer<T, V> Add(Grammer<T, V> grammer);

        void Clear();

        List<V> Parse(IEnumerable<Token<T>> tokens);

    }
}