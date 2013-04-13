using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    internal class ExtendedSymbol<T> {
        public ISymbol<T> Symbol { get; set; }
        public ItemSet<T> Start { get; set; }
        public ItemSet<T> Next { get; set; }

        public override bool Equals(object obj) {
            var sym = obj as ExtendedSymbol<T>;
            if (sym != null)
                return Symbol == sym.Symbol && Start == sym.Start && Next == sym.Next;
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Symbol.GetHashCode() ^ Start.GetHashCode() ^ (Next == null ? -1 : Next.GetHashCode());
        }

        public override string ToString() {
            return string.Format( "{0}[{1},{2}]", Symbol.ToString(), Start.SetNumber, Next == null ? -1 : Next.SetNumber);
        }
    }

    internal class ExtendedProduction<T> {
        public Production<T> Production { get; set; }
        public ExtendedSymbol<T> LHS { get; set; }
        public List<ExtendedSymbol<T>> RHS { get; set; }

        public ExtendedProduction() {
            RHS = new List<ExtendedSymbol<T>>();
        }

        public override string ToString() {
            return string.Format("{0} → {1}", LHS, string.Join(" ", from r in RHS select r.ToString()));
        }
    }
}
