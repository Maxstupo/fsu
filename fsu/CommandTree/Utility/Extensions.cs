namespace Maxstupo.Fsu.CommandTree.Utility {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class Extensions {

        public static bool IsIndexValid<T>(this T[] src, int index) {
            return !(index < 0 || index >= src.Length);
        }

        public static T Get<T>(this T[] src, int index, T defaultValue = default) {
            return src.IsIndexValid(index) ? src[index] : defaultValue;
        }

        public static string Repeat(this string str, int times) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < times; i++)
                sb.Append(str);
            return sb.ToString();
        }

        public static List<T> GetAttributes<T>(this Type type) where T : Attribute {
            return type.GetCustomAttributes(typeof(T), false).Cast<T>().ToList();
        }

        public static List<T> GetAttributes<T>(this MethodInfo method) where T : Attribute {
            return method.GetCustomAttributes(typeof(T), false).Cast<T>().ToList();
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute {
            return type.GetAttributes<T>().FirstOrDefault();
        }

        public static T GetAttribute<T>(this MethodInfo method) where T : Attribute {
            return method.GetAttributes<T>().FirstOrDefault();
        }

    }

}