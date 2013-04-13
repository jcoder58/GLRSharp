using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    internal class ReductionWorkElement<T> {
        public Production<T> Production { get; private set; }
        public List<StackNode<T>> Path { get; private set; }

        public ReductionWorkElement(Production<T> production, IEnumerable<StackNode<T>> path) {
            Production = production;
            Path = path.ToList();
        }
    }
    
}
