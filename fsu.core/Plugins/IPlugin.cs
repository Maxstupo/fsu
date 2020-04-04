namespace Maxstupo.Fsu.Core.Plugins {

    public interface IFsuPlugin {
        string PluginId { get; }
        string PluginName { get; }
        string PluginAuthor { get; }


        void Init(IFsuHost host);


        void Enable();
        void Disable();

    }

}
