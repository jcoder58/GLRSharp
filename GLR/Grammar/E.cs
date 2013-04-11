using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar {
    public class E<T> : Terminal<T> {
        public static E<T> ε = new E<T>();

        public override Match<T> Match(ISource<T> source) {
            return new Match<T>() { Start = source.Offset, Source = source, Length = 0, Terminal = this };
        }

        public override string ToString() {
            return "ε";
        }

        public override bool IsEpsilon { get { return true; } }

        public override bool IsEndOfString {
            get { return false; }
        }

    }
}
