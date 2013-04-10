using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public static class Extension {
        public static StringTerminal T(this string terminal) {
            return new StringTerminal(terminal);
        }
    }
}
