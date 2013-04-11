using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    public class Parser<T> {
        public Func<string, int, int> Skip { get; set; }
        public Action<LogLevel, string> Log { get; set; }
        public LogLevel Level { get; set; }

        NonTerminal<T> _Grammar;
        Logger _Logger;

        List<ItemSet<T>> _ItemSets;

        public Parser(NonTerminal<T> grammar)
            : this(grammar, (l, s) => { }, LogLevel.Info) {

        }

        public Parser(NonTerminal<T> grammar, Action<LogLevel, string> log , LogLevel level = LogLevel.Info) {
            Log = log;
            Level = level;
            _Logger = new Logger(() => Level, Log);

            _Grammar = new NonTerminal<T>("Γ");
            _Grammar.RHS = grammar < new EOS<T>();

            _ItemSets = ItemSet<T>.BuildAll(_Grammar, _Logger);
            FirstFollow<T> ff = new FirstFollow<T>(_ItemSets, _Logger);
        }

        public bool Parse(ISource<T> source) {
            return false;
        }



    }
}
