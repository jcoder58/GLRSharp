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

        public Parser(NonTerminal<T> grammar, Action<LogLevel, string> log, LogLevel level = LogLevel.Info) {
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
                if (lastSymbol.Symbol is EOS<T>)
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


        public bool Parse(ISource<T> source, bool lalrOnly = true) {
            if (lalrOnly)
                return LalrParser(source);
            

            return false;
        }

        private bool LalrParser(ISource<T> source) {
            bool matched = false;
            Stack<ItemSet<T>> stack = new Stack<ItemSet<T>>();
            stack.Push(_ItemSets.First());

            Match<T> match = new Match<T>() { Length = -1 };
            bool prevWasReduce = false;
            while (stack.Count > 0) {
                var itemSet = stack.Peek();
                ISource<T> prevSource = source;
                if (!prevWasReduce) {
                    foreach (var terminal in _Grammar.Terminals) {
                        var m = terminal.Match(source);
                        if (m.Success) {
                            match = m;
                            source = source.MoveTo(match.Start + match.Length);
                            if (_Logger.Debug) _Logger.LogDebug("{0}", match);
                            break;
                        }
                    }
                }
                prevWasReduce = false;
                HashSet<ItemSet<T>> shifts;
                HashSet<Production<T>> reductions;
                var shiftsFound = itemSet.Shifts.TryGetValue(match.Terminal, out shifts);
                var reducesFound = itemSet.Reductions.TryGetValue(match.Terminal, out reductions);
                if (shiftsFound != reducesFound) {
                    if (shiftsFound) {
                        if (shifts.Count > 1)
                            throw new Exception("Grammar error: shift/shift conflict");
                        stack.Push(shifts.First());
                        if (_Logger.Debug) _Logger.LogDebug("Shift to {0}", shifts.First().SetNumber);
                    } else {
                        if (reductions.Count > 1)
                            throw new Exception("Grammar error: reduce/reduce conflict");
                        var reduceProduction = reductions.First();
                        foreach (var symbol in reduceProduction.RHS)
                            stack.Pop();

                        if (match.Terminal.IsEndOfString) {
                            matched = stack.Count == 1;
                            break;
                        }
                        var top = stack.Peek();
                        var gotoItem = top.Goto[reduceProduction.LHS];
                        stack.Push(gotoItem);
                        prevWasReduce = true;
                        if (_Logger.Debug) _Logger.LogDebug("Reduce {0}, go to {1}", reduceProduction, gotoItem.SetNumber);
                    }

                } else {
                    if (!shiftsFound) {
                        matched = match.Terminal.IsEndOfString && stack.Count == 1;
                        break;
                    }



                    throw new Exception("Grammar error: shift/reduce conflict");
                }
            }

            return matched;
        }



    }
}
