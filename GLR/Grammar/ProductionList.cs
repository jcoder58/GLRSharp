using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar {
    public class ProductionList<T> : IEnumerable<Production<T>>, IList<Production<T>> {
        List<Production<T>> _Productions = new List<Production<T>>();


        public IEnumerator<Production<T>> GetEnumerator() {
            return _Productions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _Productions.GetEnumerator();
        }

        public void AddRange(ProductionList<T> list) {
            _Productions.AddRange(list);
        }

        public void Add(Production<T> item) {
            _Productions.Add(item);
        }

        public void Clear() {
            _Productions.Clear();
        }

        public bool Contains(Production<T> item) {
            return _Productions.Contains(item);
        }

        public void CopyTo(Production<T>[] array, int arrayIndex) {
            _Productions.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return _Productions.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(Production<T> item) {
            return _Productions.Remove(item);
        }

        public int IndexOf(Production<T> item) {
            return _Productions.IndexOf(item);
        }

        public void Insert(int index, Production<T> item) {
            _Productions.Insert(index, item);
        }

        public void RemoveAt(int index) {
            _Productions.RemoveAt(index);
        }

        public Production<T> this[int index] {
            get {
                return _Productions[index];
            }
            set {
                _Productions[index] = value;
            }
        }

        public static ProductionList<T> operator |(ProductionList<T> a, Production<T> b) {
            a.Add(b);
            return a;
        }

        public static ProductionList<T> operator |(ProductionList<T> a, NonTerminal<T> b) {
            a.Add(b);
            return a;
        }

        public static ProductionList<T> operator |(ProductionList<T> a, Terminal<T> b) {
            a.Add(b);
            return a;
        }

        public static implicit operator ProductionList<T>(Production<T> production) {
            return new ProductionList<T>() { production };
        }

        public static implicit operator ProductionList<T>(Terminal<T> terminal) {
            Production<T> p = new Production<T>() { terminal };
            return new ProductionList<T>() { p };
        }

        public static implicit operator ProductionList<T>(NonTerminal<T> symbol) {
            Production<T> p = new Production<T>() { symbol };
            return new ProductionList<T>() { p };
        }

        Set<Terminal<T>> _Terminals = null;
        public Set<Terminal<T>> Terminals {
            get {
                if (_Terminals == null) {
                    _Terminals = new Set<Terminal<T>>();
                    foreach (var production in this)
                        _Terminals.UnionWith(production.Terminals);
                }
                return _Terminals;
            }
        }

        Set<NonTerminal<T>> _NonTerminals = null;
        public Set<NonTerminal<T>> NonTerminals {
            get {
                if (_NonTerminals == null) {
                    _NonTerminals = new Set<NonTerminal<T>>();
                    foreach (var production in this)
                        _NonTerminals.UnionWith(production.NonTerminals);
                }
                return _NonTerminals;
            }
        }
    }
}
