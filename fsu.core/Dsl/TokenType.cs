using Maxstupo.Fsu.Core.Dsl.Lexer;

namespace Maxstupo.Fsu {
    public enum TokenType {
        Invalid,
        Eol,
        Eof,

        [TokenDef(">>", HasVariableValue = false)]
        Pipe,

        Function,   // defined dynamically.
        Constant,       // defined dynamically.

        [TokenDef(@"\@\w+")]
        ItemProperty,

        [TokenDef(@"\$\w+")]
        GlobalProperty,

        [TokenDef(@"!", HasVariableValue = false)]
        Not,

        [TokenDef(@"(?:\<|\>)=?", 2)]
        NumericOperator,

        [TokenDef(@"\>\<|\<\>")]
        [TokenDef(@"~\<|\>~")]
        StringOperator,

        [TokenDef(@"&|\|")]
        LogicOperator,

        [TokenDef(@"mb|gb|tb|kb|s|m|h|d|y", 3)]
        Unit,


        [TokenDef("\\\"([^\"]*)\\\"")]
        StringValue,

        [TokenDef(@"-?(\d+)\.(\d+)?")]
        NumberValue,

        [TokenDef(@"[\w\d:\\/\.\-\*\[\]\{\}]+", 3)]
        TextValue,


        [TokenDef(@"//")]
        Comment,

    }

}
