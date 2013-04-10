using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    public class Match<T> {
        public ISource<T> Source { get; internal set; }
        public int Start { get; internal set; }
        public int Length { get; internal set; }
        public ISymbol<T> Terminal { get; internal set; }

        public bool Success { get { return Length >= 0; } }

        public Match() {
            Length = Start = -1;
        }

        public override string ToString() {
            return string.Format("Match for {0} {2} at {1}", Terminal, Source, Success ? "succeeded" : "failed");
        }

        public override int GetHashCode() {
            return Source.GetHashCode() ^ Start.GetHashCode() ^ Length.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is Match<T>) {
                var o = obj as Match<T>;
                return Source.Equals(o.Source) & Start == o.Start && Length == o.Length;
            }

            return base.Equals(obj);
        }
    }
}
