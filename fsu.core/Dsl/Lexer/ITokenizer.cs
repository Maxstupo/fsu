using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    public interface ITokenizer<T> where T : Enum {

        void Add(TokenDefinition<T> definition);

        void Clear();

        IEnumerable<Token<T>> Tokenize(IEnumerable<string> input);

        IEnumerable<Token<T>> Tokenize(string input, int lineNumber);

    }

}