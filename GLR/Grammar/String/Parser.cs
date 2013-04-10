using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public class Parser : GLR.Parser<string> {
        public Parser(NonTerminal grammar)
            : base(grammar) {
        }

        public Parser(NonTerminal grammar, Action<LogLevel, string> log, LogLevel level = LogLevel.Info)
            : base(grammar, log, level) {
        }
    }
}
