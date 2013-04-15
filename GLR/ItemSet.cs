using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    class ItemSet<T> {
        public int SetNumber { get; private set; }
        public Set<DottedRule<T>> Rules { get; private set; }
        public Dictionary<ISymbol<T>, ItemSet<T>> Translation { get; private set; }
        public Dictionary<ISymbol<T>, ItemSet<T>> Goto { get; private set; }
        public Dictionary<ISymbol<T>, HashSet<ItemSet<T>>> Shifts { get; private set; }
        public Dictionary<ISymbol<T>, HashSet<Production<T>>> Reductions {get; private set;}

        public bool AcceptState {
            get {
                var rule = Rules.First();
                return Rules.Count == 1 && rule.Production.LHS.IsGrammarRoot && rule.AtEnd;
            }
        }

        public ItemSet(int setNumber, Set<DottedRule<T>> rules) {
            SetNumber = setNumber;
            Rules = new Set<DottedRule<T>>();
            Translation = new Dictionary<ISymbol<T>, ItemSet<T>>();
            Goto = new Dictionary<ISymbol<T>, ItemSet<T>>();
            Shifts = new Dictionary<ISymbol<T>, HashSet<ItemSet<T>>>();
            Reductions = new Dictionary<ISymbol<T>, HashSet<Production<T>>>();
            if (rules != null) {
                foreach (var rule in rules)
                    Rules.Add(rule);
            }
        }

        public bool Compare(Set<DottedRule<T>> rules) {
            return Rules.SetEquals(rules);
        }

        private static void BuildItemSet(List<ItemSet<T>> itemSets, int i, Logger logger) {
            var symbolsGroup = from rule in itemSets[i].Rules
                               where !rule.AtEnd
                               group rule by rule.Symbol into grps
                               select new {
                                   Key = grps.Key,
                                   Value = grps
                               };
            foreach (var group in symbolsGroup) {
                var ruleList = group.Value.ToList();
                var symbol = group.Key;
                var itemSet = new Set<DottedRule<T>>();
                foreach (var rule in ruleList) {
                    var next = rule.Next();
                    itemSet.Add(next);
                    foreach (var r in Items(next))
                        itemSet.Add(r);
                }

                var match = from s in itemSets where s.Compare(itemSet) select s;
                if (match.Count() == 0) {
                    itemSets.Add(new ItemSet<T>(itemSets.Count, itemSet));
                    itemSets[i].Translation.Add(symbol, itemSets.Last());
                } else {
                    itemSets[i].Translation.Add(symbol, match.First());
                }
            }
        }

        public static List<ItemSet<T>> BuildAll(NonTerminal<T> start, Logger logger) {
            var items = new List<ItemSet<T>>();

            DottedRule<T> firstItem = new DottedRule<T>(start.RHS.First(), 0);
            var firstItemSet = new Set<DottedRule<T>>();
            var lookAt = Items(firstItem).ToSet();
            firstItemSet.UnionWith(lookAt);
            items.Add(new ItemSet<T>(0, firstItemSet));

            int i = 0;
            for (i = 0; i < items.Count; i++)
                BuildItemSet(items, i, logger);

            foreach (var item in items)
                item.MakeTables();
            
            return items;
        }

        private void MakeTables() {
            foreach (var kv in Translation) {
                if (kv.Key.IsNonTerminal)
                    Goto.Add(kv.Key, kv.Value);
                else if (!kv.Key.IsEpsilon) {
                    HashSet<ItemSet<T>> set;
                    if (!Shifts.TryGetValue(kv.Key, out set))
                        Shifts.Add(kv.Key, set = new HashSet<ItemSet<T>>());
                    Shifts[kv.Key].Add(kv.Value);
                }
            }
        }

        internal void Log(Logger logger) {
            logger.LogTrace("");
            logger.LogTrace("Set {0}:", SetNumber);
            foreach (var rule in Rules)
                logger.LogTrace("\t{0}", rule.ToString());
            logger.LogTrace("Transistions");
            foreach (var transistion in Translation) {
                logger.LogTrace("{0} → {1}", transistion.Key, transistion.Value.SetNumber);
            }
            logger.LogTrace("Shifts");
            foreach (var kv in Shifts) {
                logger.LogTrace("{0}: {1}", kv.Key, ItemSetsToString(kv.Value));
            }

            logger.LogTrace("Gotos");
            foreach (var kv in Goto) {
                logger.LogTrace("{0}: {1}", kv.Key, kv.Value.SetNumber);
            }

            logger.LogTrace("Reductions");
            foreach (var kv in Reductions) {
                foreach (var r in kv.Value)
                    logger.LogTrace("{0}: {1}", kv.Key, r);
            }

        }

        public static string ItemSetsToString(HashSet<ItemSet<T>> sets) {
            var builder = new StringBuilder("[");
            foreach (var set in sets)
                builder.AppendFormat("{0}{1}", builder.Length > 1 ? "," : "", set.SetNumber);
            return builder.Append("]").ToString();
        }
        private static void LogSet(Logger logger, Set<DottedRule<T>> set, int setNumber) {
            logger.LogTrace("");
            logger.LogTrace("Set {0}:", setNumber);
            foreach (var rule in set)
                logger.LogTrace(rule.ToString());
        }

        internal static IEnumerable<DottedRule<T>> Items(DottedRule<T> rule, Set<DottedRule<T>> found = null) {
            if (found == null)
                found = new Set<DottedRule<T>>();
            yield return rule;
            if (!rule.AtEnd) {
                var symbol = rule.Symbol;
                if (symbol is NonTerminal<T>) {
                    var nt = symbol as NonTerminal<T>;

                    foreach (var production in nt.RHS) {
                        var newRule = new DottedRule<T>(production, 0);
                        if (!found.Contains(newRule)) {
                            found.Add(newRule);
                            foreach (var r in Items(newRule, found))
                                yield return r;
                        }
                    }
                }
            }
        }

    }
}
