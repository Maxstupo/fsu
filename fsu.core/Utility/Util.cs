using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxstupo.Fsu.Core.Utility {

    public static class Util {

        public static string SafeSubstring(this string str, int offset) {
            if (string.IsNullOrEmpty(str) || offset >= str.Length)
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
