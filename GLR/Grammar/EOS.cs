using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar {
    internal class EOS<T> : Terminal<T> {
        public override bool IsEpsilon {
            get { return false; }
        }

        public override Match<T> Match(ISource<T> source) {
            return new Match<T>() {
                Length = source.AtEnd ? 0 : -1,
                Source = source,
                Start = source.Offset,
                Terminal = this
            };
        }

        public override string ToString() {
            return "ω";
        }
    }
}
