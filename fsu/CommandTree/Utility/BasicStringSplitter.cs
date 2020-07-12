namespace Maxstupo.Fsu.CommandTree.Utility {
  
    using System.Collections.Generic;
    using System.Text;

    public class BasicStringSplitter : IStringSplitter {

        public string[] Split(string line, char delimiter = ' ', char ignoreToggle = '"') {
            List<string> list = new List<string>();

            StringBuilder sb = new StringBuilder();
            bool inQuote = false;

            for (int i = 0; i < line.Length; i++) {
                char c = line[i];

                if (c == ignoreToggle) {
                    inQuote = !inQuote;

                } else if (c == delimiter && !inQuote) {
                    if (sb.Length != 0) {
                        list.Add(sb.ToString());
                        sb.Clear();
                    }

                } else {
                    sb.Append(c);
                }
            }

            if (sb.Length != 0) {
                list.Add(sb.ToString());
                sb.Clear();
            }

            return list.ToArray();
        }

    }

}