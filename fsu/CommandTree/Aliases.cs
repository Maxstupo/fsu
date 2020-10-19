namespace Maxstupo.Fsu.CommandTree {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Aliases : IEnumerable<string>, IEquatable<Aliases> {

        private readonly List<string> aliases = new List<string>();

        public int Count => aliases.Count;
        public string Delimited => string.Join(", ", aliases);

        private Aliases() { }

        public static Aliases Create(params string[] aliases) {
            return new Aliases().Add(aliases);
        }

        private Aliases Add(params string[] aliases) {
            if (aliases == null)
                throw new ArgumentNullException(nameof(aliases));

            foreach (string alias in aliases) {
                if (!string.IsNullOrWhiteSpace(alias) && !Contains(alias))
                    this.aliases.Add(alias.ToLower());
            }

            return this;
        }

        public bool Contains(Aliases aliases) {
            if (aliases == null)
                throw new ArgumentNullException(nameof(aliases));

            List<string> list = new List<string>();
            list.AddRange(this.aliases);
            list.AddRange(aliases.aliases);

            return list.GroupBy(n => n).Any(c => c.Count() > 1);
        }

        public bool Contains(string alias) {
            if (alias == null)
                throw new ArgumentNullException(nameof(alias));
            return aliases.Contains(alias.ToLower());
        }

        public override bool Equals(object obj) {
            return Equals(obj as Aliases);
        }

        public bool Equals(Aliases other) {
            return other != null &&
                   EqualityComparer<List<string>>.Default.Equals(this.aliases, other.aliases);
        }

        public override int GetHashCode() {
            return 1905762551 + EqualityComparer<List<string>>.Default.GetHashCode(this.aliases);
        }

        public static bool operator ==(Aliases left, Aliases right) {
            return EqualityComparer<Aliases>.Default.Equals(left, right);
        }

        public static bool operator !=(Aliases left, Aliases right) {
            return !(left == right);
        }

        public IEnumerator<string> GetEnumerator() {
            return aliases.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}