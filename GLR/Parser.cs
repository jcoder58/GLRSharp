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

            MakeReductions(ff);

            if (_Logger.Trace) {
                foreach (var itemSet in _ItemSets)
                    itemSet.Log(_Logger);
            }
        }

        private void MakeReductions(FirstFollow<T> ff) {
            foreach (var production in ff.ExtendedGrammar) {
                var lastSymbol = production.RHS.Last();
                if (lastSymbol.Symbol is EOS<T> )
                    continue;
                HashSet<ISymbol<T>> follow;
                if (!ff.Follow.TryGetValue(lastSymbol, out follow))
                    follow = ff.Follow[production.LHS];
                var endSet = lastSymbol.Next;
                foreach (var symbol in follow) {
                    HashSet<Production<T>> r;
                    if (!endSet.Reductions.TryGetValue(symbol, out r))
                        endSet.Reductions.Add(symbol, r = new HashSet<Production<T>>());
                   r.Add(production.Production);
                }
            }
        }

        public bool Parse(ISource<T> source) {
            return false;
        }



    }
}
