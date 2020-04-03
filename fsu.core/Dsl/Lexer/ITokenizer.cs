using System;
using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Dsl.Lexer {

    public interface ITokenizer<T> where T : Enum {

        T EndOfFileToken { get; }

        T EndOfLineToken { get; }

        T InvalidToken { get; }


        void AddDef(TokenDefinition<T> definition);

        void Clear();


        IEnumerable<Token<T>> Parse(IEnumerable<string> input);

        IEnumerable<Token<T>> Parse(string input, int lineNumber);

    }

}