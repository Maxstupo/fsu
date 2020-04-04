using System.Reflection;

namespace Maxstupo.Fsu.Core.Plugins {
    public interface IPluginManager {
        IFsuPlugin GetPlugin(string pluginId);
        IFsuPlugin GetPluginByName(string pluginName);
        IFsuPlugin GetPluginByName(string pluginName, string pluginAuthor);
        bool LoadPluginAssembly(string filepath);
        void LoadPluginFromAssembly(Assembly assembly);
        bool LoadPluginsFromDirectory(string directory);
    }
}