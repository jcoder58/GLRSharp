using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public abstract class Terminal : GLR.Grammar.Terminal<string> {
        public static Terminal T(string t) { return new StringTerminal(t); }

        public static implicit operator Terminal(string t) {
            return new StringTerminal(t);
        }
    }
}
