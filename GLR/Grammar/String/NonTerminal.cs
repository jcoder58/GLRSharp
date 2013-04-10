using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public class NonTerminal : GLR.Grammar.NonTerminal<string> {
        public NonTerminal(string name = "")
            : base(name) {
        }

        public override string ToString() {
            return base.ToString();
        }
    }
}
