using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {

    internal class Path<T> {
        internal StackNode<T> Node { get; set; }
        internal StackLink<T> LinkToParent { get; set; }

        public override string ToString() {
            return Node.ToString();
        }
    }
    
    
    internal class ReductionWorkElement<T> {
        public Production<T> Production { get; private set; }
        public List<Path<T>> Path { get; private set; }

        public ReductionWorkElement(Production<T> production, IEnumerable<Path<T>> path) {
            Production = production;
            Path = path.ToList();
        }
    }
    
}
