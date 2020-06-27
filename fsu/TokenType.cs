namespace Maxstupo.Fsu {

    using Maxstupo.Fsu.Core.Dsl.Lexer;

    public enum TokenType {
        Invalid,
        Eol,
        Eof,

        [TokenDef(">>", HasVariableValue = false)]
        Pipe,

        Function,   // defined dynamically in FsuLanuageSpec.cs
        Constant,   // defined dynamically in FsuLanuageSpec.cs

        [TokenDef(@"\@\{\w+\}")]
        ItemProperty,

        [TokenDef(@"\$\{\w+\}")]
        GlobalProperty,

        [TokenDef(@"!", HasVariableValue = false)]
        Not,

        [TokenDef(@"(?:(?:\<|\>)=?)|=", 2)]
        NumericOperator,

        [TokenDef(@"\>\<|\<\>")]
        [TokenDef(@"~\<|\>~")]
        StringOperator,

        [TokenDef(@"&|\|")]
        LogicOperator,

        [TokenDef(@"mb|gb|tb|kb|s|m|h", 3)]
        Unit,

        [TokenDef("\"((?:\\\\.|[^\\\\\"])*)\"", RemoveRegex = "\\\\(?=\\\")")]
        StringValue,

        [TokenDef(@"-?(?:\d+)\.(?:\d+)?")]
        NumberValue,

        [TokenDef(@"[\w\d:\\/\.\-\*\[\]\{\}]+", 3)]
        TextValue,

        [TokenDef(@",")]
        Seperator,

        [TokenDef(@"//")]
        Comment,

    }

}
