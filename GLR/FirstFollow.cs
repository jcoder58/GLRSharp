using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    internal class FirstFollow<T> {
        List<ExtendedProduction<T>> _ExtendedGrammar = new List<ExtendedProduction<T>>();
        List<ItemSet<T>> _ItemSets;

        internal List<ExtendedProduction<T>> ExtendedGrammar { get { return _ExtendedGrammar; } }
        internal Dictionary<ExtendedSymbol<T>, HashSet<ISymbol<T>>> First { get; private set; }
        internal Dictionary<ExtendedSymbol<T>, HashSet<ISymbol<T>>> Follow { get; private set; }
        Logger _Logger;

        internal FirstFollow(List<ItemSet<T>> itemSets, Logger logger) {
            _ItemSets = itemSets;
            _Logger = logger;

            First = new Dictionary<ExtendedSymbol<T>, HashSet<ISymbol<T>>>();
            Follow = new Dictionary<ExtendedSymbol<T>, HashSet<ISymbol<T>>>();

            MakeExtendGrammar();
            CalculateFirst();
            CalculateFollow();
        }

        private void CalculateFollow() {
            bool changed = false;
            do {
                changed = false;
                foreach (var production in _ExtendedGrammar) {
                    var lastSymbol = production.RHS[production.RHS.Count - 1];
                    if (lastSymbol.Symbol.IsNonTerminal && !Follow[lastSymbol].IsSupersetOf(Follow[production.LHS])) {
                        Follow[lastSymbol].UnionWith(Follow[production.LHS]);
                        changed = true;
                    }
                    for (int i = 0; i < production.RHS.Count - 1; i++) {
                        var symbol = production.RHS[i];
                        if (symbol.Symbol.IsNonTerminal) {
                            for (int j = i + 1; j < production.RHS.Count; j++) {
                                var nextSymbol = production.RHS[j];
                                if (nextSymbol.Symbol.IsNonTerminal) {
                                    if (!Follow[symbol].IsSupersetOf(First[nextSymbol])) {
                                        Follow[symbol].UnionWith(First[nextSymbol]);
                                        changed = true;
                                    }
                                } else if (!Follow[symbol].Contains(nextSymbol.Symbol)) {
                                    Follow[symbol].Add(nextSymbol.Symbol);
                                    changed = true;
                                }
                                if (!nextSymbol.Symbol.IsNullable)
                                    break;
                            }
                        }

                    }
                }
            } while (changed);

            foreach (var kv in Follow) {
                var nullTerminals = (from f in kv.Value where f.IsEpsilon select f).ToList();
                foreach (var nt in nullTerminals)
                    kv.Value.Remove(nt);
            }

            if (_Logger.Trace) {
                _Logger.LogTrace("Extended Follow");
                foreach (var follow in Follow) {
                    StringBuilder builder = ShowSet(follow);
                    _Logger.LogTrace(builder.ToString());
                }
            }
        }

        private void CalculateFirst() {

            bool changed = false;
            do {
                changed = false;
                foreach (var production in _ExtendedGrammar) {
                    var first = First[production.LHS];
                    foreach (var symbol in production.RHS) {
                        if (symbol.Symbol.IsNonTerminal) {
                            var first2 = First[symbol];
                            if (!first.IsSupersetOf(first2)) {
                                first.UnionWith(first2);
                                changed = true;
                            }
                        } else {
                            if (!first.Contains(symbol.Symbol)) {
                                first.Add(symbol.Symbol);
                                changed = true;
                            }
                        }

                        if (!symbol.Symbol.IsNullable)
                            break;
                    }
                }
            } while (changed);

            if (_Logger.Trace) {
                _Logger.LogTrace("Extended First");
                foreach (var first in First) {
                    StringBuilder builder = ShowSet(first);
                    _Logger.LogTrace(builder.ToString());
                }
            }
        }

        private void MakeExtendGrammar() {
            var firstRules = from item in _ItemSets
                             from rule in item.Rules
                             where rule.Dot == 0
                             select new { Item = item, Rule = rule };

            foreach (var ir in firstRules) {
                var parentRule = ir.Rule;
                var parentItem = ir.Item;
                var rule = ir.Rule;
                var rhsSet = ir.Item;
                ExtendedSymbol<T> extendedLHS = new ExtendedSymbol<T>() { Symbol = parentRule.Production.LHS, Start = parentItem };
                ExtendedProduction<T> production = new ExtendedProduction<T>() { LHS = extendedLHS, Production = rule.Production };
                while (!rule.AtEnd) {
                    ExtendedSymbol<T> symbol = new ExtendedSymbol<T>() { Start = rhsSet, Symbol = rule.Symbol };
                    production.RHS.Add(symbol);
                    rhsSet = rhsSet.Goto[rule.Symbol];
                    symbol.Next = rhsSet;
                    rule = rule.Next();
                }
                var RHSItem = rhsSet;
                ItemSet<T> lhsSet = null;
                parentItem.Goto.TryGetValue(parentRule.Production.LHS, out lhsSet);
                extendedLHS.Next = lhsSet;
                _ExtendedGrammar.Add(production);
                if (!First.ContainsKey(extendedLHS)) {
                    First.Add(extendedLHS, new HashSet<ISymbol<T>>());
                    Follow.Add(extendedLHS, new HashSet<ISymbol<T>>());
                }
            }

            if (_Logger.Trace) {
                _Logger.LogTrace("Extended Productions: ");
                foreach (var production in _ExtendedGrammar) {
                    StringBuilder builder = new StringBuilder();
                    Add(builder, production.LHS);

                    builder.Append(" → ");

                    foreach (var symbol in production.RHS) {
                        Add(builder, symbol);
                        builder.Append(" ");
                    }
                    _Logger.LogTrace(builder.ToString());
                }
                _Logger.LogTrace("");
            }
        }

        private StringBuilder ShowSet(KeyValuePair<ExtendedSymbol<T>, HashSet<ISymbol<T>>> follow) {
            StringBuilder builder = new StringBuilder();
            Add(builder, follow.Key);
            builder.Append("[");
            foreach (var f in follow.Value) {
                if (f != follow.Value.First())
                    builder.Append(",");
                builder.Append(f.ToString());
            }
            builder.Append("]");
            return builder;
        }
        
        private void Add(StringBuilder builder, ExtendedSymbol<T> extendedSymbol) {
            builder.AppendFormat("{0}[{1},{2}]", extendedSymbol.Symbol.ToString(),
                extendedSymbol.Start.SetNumber,
                extendedSymbol.Next == null ? "$" : extendedSymbol.Next.SetNumber.ToString());
        }
    }

}
