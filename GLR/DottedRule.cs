using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLR.Grammar;

namespace GLR {
    class DottedRule<T> {
        public Production<T> Production { get; private set; }
        public int Dot { get; private set; }

        public ISymbol<T> Symbol { get { return Production[Dot]; } }

        public DottedRule(Production<T> production, int dot) {
            Production = production;
            Dot = dot;
        }

        public bool AtEnd { get { return Dot == Production.Count; } }

        public DottedRule<T> Next() {
            if (AtEnd)
                return this;
            return new DottedRule<T>(Production, Dot + 1);
        }

        public override string ToString() {
            return Production.ToString(Dot);
        }

        public override bool Equals(object obj) {
            if (obj is DottedRule<T>) {
                var dr = obj as DottedRule<T>;
                return Dot == dr.Dot && Production.Equals(dr.Production);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Production.GetHashCode() ^ Dot.GetHashCode();
        }
    }
}
