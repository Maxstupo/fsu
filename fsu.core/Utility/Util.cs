using System;

namespace Maxstupo.Fsu.Core.Utility {

    /// <summary>
    /// Provides basic static utility methods and extensions for various purposes. 
    /// </summary>
    public static class Util {

        /// <summary>
        /// Returns a substring from this instance. Functions like <see cref="string.Substring(int)"/> but permits an out of bounds startIndex.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index from this string instance.</param>
        /// <returns>A substring, or the original string if the index was out of bounds.</returns>
        public static string SafeSubstring(this string str, int startIndex) {
            if (str.Length == 0 || startIndex < 0 || startIndex >= str.Length)
                return str;

            return str.Substring(startIndex);
        }

        public static bool Contains(this string src, string value, StringComparison comparison) {
            return src.IndexOf(value, comparison) >= 0;
        }

    }

}