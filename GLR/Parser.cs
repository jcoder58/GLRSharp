using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    public class Parser<T> {
        public Func<T, int, int> Skip { get; set; }
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
            Skip = (t, i) => { return i; };

            _Grammar = new NonTerminal<T>("Γ") { IsGrammarRoot = true };
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
                if (lastSymbol.Symbol.IsEndOfString)
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


        public bool Parse(ISource<T> source, bool lalrOnly = false) {
            if (lalrOnly)
                return LALRParser(source);

            return GLRParser(source); ;
        }

        public IList<object> Matches {
            get {
                return (from top in _Tops
                       from link1 in top.Links
                       from link2 in link1.Child.Links
                        select link2.Value).ToList<object>();
            }
        }

        List<StackNode<T>> _Tops = new List<StackNode<T>>();
        Queue<ReductionWorkElement<T>> _ReductionWorkQueue = new Queue<ReductionWorkElement<T>>();
        private bool GLRParser(ISource<T> source) {
            bool matched = false;
            _Tops.Clear();
            _ReductionWorkQueue.Clear();

            if (_Logger.Debug) _Logger.LogDebug("Parsing {0}", source);
            _Tops.Add(new StackNode<T>(_ItemSets.First(), null, null));
            Match<T> match = new Match<T>() { Length = -1 };
            while (true) {
                if (!GetNextToken(ref source, ref match))
                    break;
                if (_Logger.Trace)_Logger.LogTrace("R Top: {0}", string.Join(",", _Tops));
                DoReductions(match);
                if (_Logger.Trace) _Logger.LogTrace("S Top: {0}", string.Join(",", _Tops));
                DoShifts(match);
                if (match.Terminal.IsEndOfString) {
                    if (_Logger.Trace) _Logger.LogTrace("EOS Top: {0}", string.Join(",", _Tops));
                    _Tops.RemoveAll(top => !top.ItemSet.AcceptState);
                    matched = _Tops.Count > 0;
                    break;
                }
            }
            return matched;
        }

        private void ReducePath(ReductionWorkElement<T> path, Match<T> match) {
            var attributes = (from p in path.Path where p.LinkToParent != null select p.LinkToParent.Value).Reverse().ToList<object>();
            var value = path.Production.Action(attributes);
            if (_Logger.Debug) _Logger.LogDebug("Production: {0} Value {1}", path.Production, value);
            var leftSib = path.Path.Last();
            var rightSib = (from node in _Tops
                            where node.ItemSet.Goto.ContainsKey(path.Production.LHS) &&
                            node.ItemSet.Goto[path.Production.LHS] == leftSib.Node.ItemSet
                            select node).FirstOrDefault();
            if (rightSib != null) {
                throw new NotImplementedException();
            } else {
                rightSib = new StackNode<T>(leftSib.Node.ItemSet.Goto[path.Production.LHS], leftSib.Node, value);
                _Tops.Add(rightSib);
                if (_Logger.Debug) _Logger.LogDebug("   Reduce {0} Goto {1}", path.Production, rightSib);

                // Any more reductions from new sibling
                EnqueueReductions(match, rightSib);
            }
        }

        private void DoShifts(Match<T> match) {
            List<StackNode<T>> previousTops = _Tops;
            _Tops = new List<StackNode<T>>();
            var shiftNodes = from node in previousTops
                             where node.ItemSet.Shifts.ContainsKey(match.Terminal)
                             select node;
            foreach (var node in shiftNodes) {
                var merges = from t in _Tops
                             where t.ItemSet == node.ItemSet
                             select t;
                if (merges.Count() > 0) {
                    foreach (var merge in merges) {
                        merge.Links.Add(new StackLink<T>() { Parent = merge, Child = node, Value = null });
                    }
                } else {
                    foreach (var itemSet in node.ItemSet.Shifts[match.Terminal]) {
                        StackNode<T> newNode = new StackNode<T>(itemSet, node, match.Terminal.Value);
                        _Tops.Add(newNode);
                        if (_Logger.Debug) _Logger.LogDebug("   Shift node {0}", itemSet.SetNumber);
                    }
                }
            }
        }

        private void DoReductions(Match<T> match) {
            EnqueueReductions(match);

            while (_ReductionWorkQueue.Count > 0) {
                var tryReduction = _ReductionWorkQueue.Dequeue();
                if (_Logger.Trace) _Logger.LogTrace("   Dequeue Path {{{0}}} for {1}", ShowPath(tryReduction.Path), tryReduction.Production);
                ReducePath(tryReduction, match);
            }
        }

        private void EnqueueReductions(Match<T> match, StackNode<T> onlyNode=null) {
            IEnumerable<StackNode<T>> lookAt = _Tops;
            if (onlyNode != null)
                lookAt = new[] { onlyNode };
            var reductions = from node in lookAt 
                             where node.ItemSet.Reductions.ContainsKey(match.Terminal)
                             select node;
            foreach (var node in reductions) {
                foreach (var reduction in node.ItemSet.Reductions[match.Terminal]) {
                    var paths = Paths(node, reduction);
                    foreach (var path in paths) {
                        _ReductionWorkQueue.Enqueue(new ReductionWorkElement<T>(reduction, path));
                        if (_Logger.Trace) _Logger.LogTrace("   Enqueue Path {{{0}}} for {1}", ShowPath(path), reduction);
                    }
                }
            }
        }

        private IEnumerable<IEnumerable<Path<T>>> Paths(StackNode<T> node, int countLeft) {
            if (countLeft >= 0) {
                foreach (var link in node.Links)
                    foreach (var n in Paths(link.Child, countLeft - 1))
                        yield return new[] {new Path<T>() { Node= link.Child, LinkToParent = link } }.Concat(n);
            } else
                yield return new Path<T>[0];
        }

        private IEnumerable<IEnumerable<Path<T>>> Paths(StackNode<T> top, Production<T> production) {
            var count = production.RHS.Count - 1;
            foreach (var path in Paths(top, count))
                yield return (new[] { new Path<T>() { Node= top, LinkToParent = null} }.Concat(path)).ToArray();
        }



        private string ShowPath(IEnumerable<Path<T>> path) {
            return string.Join(",", from p in path select p.ToString());
        }

        private bool GetNextToken(ref ISource<T> source, ref Match<T> match) {
            match.Length = -1;
            var skipSource = source.Skip(Skip);
            foreach (var terminal in _Grammar.Terminals) {
                var m = terminal.Match(skipSource);
                if (m.Success) {
                    match = m;
                    source = source.MoveTo(match.Start + match.Length);
                    if (_Logger.Debug) _Logger.LogDebug("{0}", match);
                    break;
                }
            }
            return match.Success;
        }


        private bool LALRParser(ISource<T> source) {
            bool matched = false;
            Stack<ItemSet<T>> stack = new Stack<ItemSet<T>>();
            stack.Push(_ItemSets.First());

            Match<T> match = new Match<T>() { Length = -1 };
            bool nextToken = true;
            while (stack.Count > 0) {
                var itemSet = stack.Peek();
                ISource<T> prevSource = source;
                if (nextToken) {
                    GetNextToken(ref source, ref match);
                }
                nextToken = true;
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
                        nextToken = false;
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
