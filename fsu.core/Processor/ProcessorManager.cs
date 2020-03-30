using Maxstupo.Fsu.Core.Lex;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maxstupo.Fsu.Core.Processor {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class Function : Attribute {
        public string Keyword { get; }

        public Function(string keyword) {
            Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        }
    }

    public class ProcessorManager {

        private static ProcessorManager instance = null;
        public static ProcessorManager Instance => instance ?? (instance = new ProcessorManager());

        private readonly Dictionary<string, Type> lookup = new Dictionary<string, Type>();

        public List<string> Names => lookup.Keys.ToList();

        private ProcessorManager() {
            //    LoadProcessors();
        }

        public void LoadProcessors(bool silent = false) {
            lookup.Clear();

            Type[] types = AppDomain.CurrentDomain.GetAssemblies().
                                            SelectMany(x => x.GetTypes()).
                                            Where(x => typeof(IProcessor).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).
                                            ToArray();

            foreach (Type type in types) {
                Function attr = type.GetCustomAttribute<Function>(false);
                if (attr == null)
                    continue;

                if (lookup.ContainsKey(attr.Keyword))
                    throw new Exception($"A processor with the name '{attr.Keyword}' is already registered!");

                MethodInfo info = type.GetMethod("Construct", BindingFlags.Public | BindingFlags.Static);
                if (info == null || info.ReturnType != typeof(object) || info.GetParameters().Length != 2)
                    throw new Exception($"Processor missing static Construct(TokenList) - {type.Name}");


                lookup.Add(attr.Keyword, type);
                if (!silent)
                    ColorConsole.WriteLine($"&-7;Loaded: &-e;{attr.Keyword,-10}&-^; ({type.FullName})&-^;");
            }
        }

        public bool Construct(string keyword, TokenList tokenList, out IProcessor processor, out string error) {
            Type type = lookup[keyword];

            MethodInfo info = type.GetMethod("Construct", BindingFlags.Public | BindingFlags.Static);

            if (info == null || info.ReturnType != typeof(object) || info.GetParameters().Length != 2) {
                throw new Exception($"Processor missing static Construct(TokenList) - {type.Name}");

            } else {

                object result = info.Invoke(null, new object[] { keyword, tokenList });

                if (result is string str) {
                    error = str;
                    processor = null;
                    return false;
                } else {
                    error = null;
                    processor = (result as IProcessor) ?? throw new Exception($"Processor Construct(TokenList) must return an instance of IProcessor or an error string: {type.Name}");
                    return true;
                }

            }

        }

    }

}
