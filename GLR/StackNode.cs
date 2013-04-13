using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR {
    internal class StackNode<T> {
        public ItemSet<T> ItemSet { get; private set; }
        public List<StackNode<T>> Nodes { get; private set; }

        public StackNode(ItemSet<T> items, StackNode<T> left = null ) {
            ItemSet = items;
            Nodes = new List<StackNode<T>>();
            if (left != null)
                Nodes.Add(left);
        }

        public override string ToString() {
            return string.Format("Node {0} [{1}]", ItemSet.SetNumber,
                string.Join(",", from n in Nodes select n.ItemSet.SetNumber.ToString()));
        }
    }
}
