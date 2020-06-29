namespace Maxstupo.Fsu.Core.Plugins {
    
    using System.Collections.Generic;
    using Maxstupo.Fsu.Core.Dsl.Lexer;
    using Maxstupo.Fsu.Core.Dsl.Parser;
    using Maxstupo.Fsu.Core.Processor;

    public interface IFsuPlugin {

        ISet<Grammer<TokenType, IProcessor>> GetPluginGrammers();

        ISet<TokenDefinition<TokenType>> GetPluginTokenDefinitions();

    }

    public abstract class FsuPlugin : IFsuPlugin {

        public virtual ISet<Grammer<TokenType, IProcessor>> GetPluginGrammers() {
            return null;
        }

        public virtual ISet<TokenDefinition<TokenType>> GetPluginTokenDefinitions() {
            return null;
        }

    }

}
