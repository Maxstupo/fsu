namespace Maxstupo.Fsu.Core.Utility {

    using System;

    public static class Util {

        public static void Populate<T>(this T[] arr, T value) {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = value;
        }

        public static string SafeSubstring(this string str, int offset) {
            if (string.IsNullOrEmpty(str) || offset < 0 || offset >= str.Length)
                return str;

            return str.Substring(offset);
        }

        public static bool Contains(this string src, string value, StringComparison comparison) {
            return src.IndexOf(value, comparison) >= 0;
        }

        public static bool ContainsAny(this string src, string[] values, StringComparison comparison) {
            foreach (string value in values) {
                if (src.Contains(value, comparison))
                    return true;
            }
            return false;
        }

    }

}