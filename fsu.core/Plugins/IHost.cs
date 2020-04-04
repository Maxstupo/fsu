using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Dsl.Lexer;
using Maxstupo.Fsu.Core.Dsl.Parser;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;

namespace Maxstupo.Fsu.Core.Plugins {

    public interface IFsuHost {

        IPluginManager PluginManager { get; }

        IConsole Console { get; }

        ITokenizer<TokenType> Tokenizer { get; }

        ITokenParser<TokenType, IProcessor> Parser { get; }

        IPropertyProviderList PropertyProviders { get; }
    }

}
