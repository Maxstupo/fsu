namespace Maxstupo.Fsu.CommandTree {
    using System.Collections.Generic;

    public abstract class Node<T> where T : Node<T> {

        private T parent = null;
        public T Parent {
            get => parent;
            private set {
                if (parent != null)
                    parent.children.Remove(this);

                parent = value;

                if (parent != null)
                    parent.children.Add(this);
            }
        }

        private readonly List<Node<T>> children = new List<Node<T>>();

        public IReadOnlyList<Node<T>> Children => children.AsReadOnly();

        public bool IsRoot => parent == null;
        public bool IsLeaf => children.Count == 0;

        public Node<T> Root => !IsRoot ? Parent.Root : this;

        public Node(T parent = null) {
            Parent = parent;
        }

    }

}