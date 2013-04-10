using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar;

namespace GLR {
    class ItemSet<T> {
        public int SetNumber { get; private set; }
        public Set<DottedRule<T>> Rules { get; private set; }
        public Dictionary<ISymbol<T>, ItemSet<T>> Transistions { get; private set; }

        public ItemSet(int setNumber, Set<DottedRule<T>> rules) {
            SetNumber = setNumber;
            Rules = new Set<DottedRule<T>>();
            Transistions = new Dictionary<ISymbol<T>, ItemSet<T>>();
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
                var ruleList = group.Value;
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
                    itemSets[i].Transistions.Add(symbol, itemSets.Last());
                } else {
                    itemSets[i].Transistions.Add(symbol, match.First());
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

            if (logger.Trace)
                foreach (var item in items)
                    item.Log(logger);

            return items;
        }

        private void Log(Logger logger) {
            logger.LogTrace("");
            logger.LogTrace("Set {0}:", SetNumber);
            foreach (var rule in Rules)
                logger.LogTrace("\t{0}", rule.ToString());
            foreach (var transistion in Transistions) {
                logger.LogTrace("{0} → {1}", transistion.Key, transistion.Value.SetNumber);
            }

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
