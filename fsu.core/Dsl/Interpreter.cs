namespace Maxstupo.Fsu.Core.Dsl {

    using System;
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;

    public class Interpreter<T, V> : IInterpreter<V> where T : Enum where V : class {

        private readonly ITokenizer<T> tokenizer;
        private readonly ITokenParser<T, V> parser;

        public Interpreter(ITokenizer<T> tokenizer, ITokenParser<T, V> parser) {
            this.tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public List<V> Eval(params string[] text) {
            IEnumerable<Token<T>> tokens = tokenizer.Tokenize(text);
            return parser.Parse(tokens);
        }

    }

}