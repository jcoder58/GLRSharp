using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR {
    internal class StackLink<T> {
        public StackNode<T> Parent { get; set; }
        public StackNode<T> Child { get; set; }
        public object Value { get; set; }
    }

    internal class StackNode<T> {
        public ItemSet<T> ItemSet { get; private set; }
        public List<StackLink<T>> Links { get; private set; }

        public StackNode(ItemSet<T> items, StackNode<T> left, object value) {
            ItemSet = items;
            Links = new List<StackLink<T>>();
            if (left != null)
                Links.Add(new StackLink<T>() { Parent=this, Child = left, Value = value});
        }

        public override string ToString() {
            return string.Format("Node {0} Links [{1}]", ItemSet.SetNumber,
                string.Join(",", from link in Links select string.Format("({0},{1})", link.Child.ItemSet.SetNumber.ToString(), link.Value ?? "null") ));
        }
    }
}
