using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Maxstupo.Fsu.Core.Plugins {

    public class PluginManager : IPluginManager {

        private readonly IFsuHost host;
        private readonly Dictionary<string, IFsuPlugin> plugins = new Dictionary<string, IFsuPlugin>();

        public PluginManager(IFsuHost host) {
            this.host = host;
        }

        public IFsuPlugin GetPlugin(string pluginId) {
            return plugins.TryGetValue(pluginId, out IFsuPlugin plugin) ? plugin : null;
        }

        public IFsuPlugin GetPluginByName(string pluginName) {
            return plugins.First(x => x.Value.PluginName.Equals(pluginName, StringComparison.InvariantCulture)).Value;
        }

        public IFsuPlugin GetPluginByName(string pluginName, string pluginAuthor) {
            return plugins.First(x => x.Value.PluginName.Equals(pluginName, StringComparison.InvariantCulture) && x.Value.PluginAuthor.Equals(pluginAuthor, StringComparison.InvariantCulture)).Value;
        }

        public bool LoadPluginsFromDirectory(string directory) {
            if (!Directory.Exists(directory))
                return false;

            foreach (string filepath in Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories))
                LoadPluginAssembly(filepath);

            return true;
        }

        public bool LoadPluginAssembly(string filepath) {
            if (!File.Exists(filepath))
                return false;

            Assembly assembly = Assembly.LoadFrom(filepath);
            LoadPluginFromAssembly(assembly);

            return true;
        }

        public void LoadPluginFromAssembly(Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            Type type = assembly.GetTypes().Where(x => typeof(IFsuPlugin).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface).FirstOrDefault();

            if (type != null) {
                IFsuPlugin plugin = (IFsuPlugin) Activator.CreateInstance(type);

                if (!plugins.ContainsKey(plugin.PluginId)) {
                    plugins.Add(plugin.PluginId, plugin);

                    host.Console.WriteLine($"Loading plugin: '{plugin.PluginName}' ({plugin.PluginAuthor})...");
                    plugin.Init(host);
                    host.Console.WriteLine($"Loaded.");

                    plugin.Enable();

                } else {
                    host.Console.WriteLine($"Plugin already loaded: '{plugin.PluginName}' ({plugin.PluginAuthor})...");
                }
            }
        }


    }



}
