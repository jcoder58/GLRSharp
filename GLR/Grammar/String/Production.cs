using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public class Production : GLR.Grammar.Production<string> {
        public static implicit operator Production(Terminal terminal) {
            return new Production() { terminal };
        }

        public static implicit operator Production(NonTerminal nt) {
            return new Production() { nt };
        }

    }
}
