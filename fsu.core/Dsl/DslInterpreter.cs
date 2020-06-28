namespace Maxstupo.Fsu.Core.Dsl {

    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;
    using System;
    using System.Collections.Generic;

    public class DslInterpreter<T, V> : IDslInterpreter<V> where T : Enum where V : class {

        private readonly ITokenizer<T> tokenizer;
        private readonly ITokenParser<T, V> parser;

        public DslInterpreter(ITokenizer<T> tokenizer, ITokenParser<T, V> parser) {
            this.tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public List<V> Eval(params string[] text) {
            IEnumerable<Token<T>> tokens = tokenizer.Tokenize(text);
            return parser.Parse(tokens);
        }

    }

}