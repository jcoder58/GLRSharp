using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace GLR.Grammar {
    public class Production<T> : IList<ISymbol<T>> {
        private NonTerminal<T> _LHS = null;
        public NonTerminal<T> LHS {
            get { return _LHS; }
            internal set { Debug.Assert(_LHS == null); _LHS = value; }
        }

        public Production<T> this[Func<IList<object>, object> index] {
            get {
                return this;
            }
            set {
                Action = index;
            }
        }
        public Func<IList<object>, object> Action { get; set; }

        public ReadOnlyCollection<ISymbol<T>> RHS { get { return new ReadOnlyCollection<ISymbol<T>>(_Symbols); } }

        private List<ISymbol<T>> _Symbols = new List<ISymbol<T>>();


        public int IndexOf(ISymbol<T> item) {
            return _Symbols.IndexOf(item);
        }

        public void Insert(int index, ISymbol<T> item) {
            _Symbols.Insert(index, item);
        }

        public void RemoveAt(int index) {
            _Symbols.RemoveAt(index);
        }

        public ISymbol<T> this[int index] {
            get {
                return _Symbols[index];
            }
            set {
                _Symbols[index] = value;
            }
        }

        public void Add(ISymbol<T> item) {
            _Symbols.Add(item);
        }

        public void AddRange(IEnumerable<ISymbol<T>> items) {
            _Symbols.AddRange(items);
        }

        public void Clear() {
            _Symbols.Clear();
        }

        public bool Contains(ISymbol<T> item) {
            return _Symbols.Contains(item);
        }

        public void CopyTo(ISymbol<T>[] array, int arrayIndex) {
            _Symbols.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return _Symbols.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(ISymbol<T> item) {
            return _Symbols.Remove(item);
        }

        public IEnumerator<ISymbol<T>> GetEnumerator() {
            return _Symbols.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _Symbols.GetEnumerator();
        }

        #region Operators
        public static ProductionList<T> operator |(Production<T> a, NonTerminal<T> b) {
            return new ProductionList<T>() { a, b };
        }

        public static ProductionList<T> operator |(Production<T> a, Terminal<T> b) {
            return new ProductionList<T>() { a, b };
        }

        public static ProductionList<T> operator |(Production<T> a, Production<T> b) {
            return new ProductionList<T>() { a, b };
        }

        public static Production<T> operator <(Production<T> a, Production<T> b) {
            a.AddRange(b);
            return a;
        }

        public static Production<T> operator >(Production<T> a, Production<T> b) {
            return a < b;
        }

        public static Production<T> operator <(Production<T> a, Terminal<T> b) {
            return a < (Production<T>)b;
        }

        public static Production<T> operator >(Production<T> a, Terminal<T> b) {
            return a < b;
        }

        public static Production<T> operator <(Production<T> a, NonTerminal<T> b) {
            return a < (Production<T>)b;
        }

        public static Production<T> operator >(Production<T> a, NonTerminal<T> b) {
            return a < b;
        }

        #endregion

        public static implicit operator Production<T>(Terminal<T> terminal) {
            return new Production<T>() { terminal };
        }

        public static implicit operator Production<T>(NonTerminal<T> nt) {
            return new Production<T>() { nt };
        }


        public override string ToString() {
            StringBuilder b = new StringBuilder();
            b.AppendFormat("{0}{1} → ", LHS.Name, LHS.IsNullable ? "*" : "");
            foreach (var r in RHS)
                b.Append(r);
            return b.ToString();
        }

        public string ToString(int offset) {
            StringBuilder b = new StringBuilder();
            if (LHS != null)
                b.AppendFormat("{0}{1} → ", LHS.Name, LHS.IsNullable ? "*" : "");
            else
                b.Append("? → ");

            for (int i = 0; i < RHS.Count; i++) {
                var s = RHS[i];
                if (offset == i)
                    b.Append('●');
                if (s is NonTerminal<T>)
                    b.Append((s as NonTerminal<T>).Name).Append(" ");
                else
                    b.Append(s).Append(" ");
            }
            if (offset >= RHS.Count)
                b.Append('●');
            return b.ToString();
        }


        Set<Terminal<T>> _Terminals = null;
        public Set<Terminal<T>> Terminals {
            get {
                if (_Terminals == null) {
                    _Terminals = new Set<Terminal<T>>();
                    foreach (var symbol in RHS)
                        _Terminals.UnionWith(symbol.Terminals);
                }
                return _Terminals;
            }
        }

        Set<NonTerminal<T>> _NonTerminals = null;
        public Set<NonTerminal<T>> NonTerminals {
            get {
                if (_NonTerminals == null) {
                    _NonTerminals = new Set<NonTerminal<T>>();
                    foreach (var symbol in RHS)
                        _NonTerminals.UnionWith(symbol.NonTerminals);
                }
                return _NonTerminals;
            }
        }

        internal void Visit(Action<ISymbol<T>> action, Set<ISymbol<T>> visited) {
            foreach (var symbol in this)
                symbol.Visit(action, visited);
        }
    }

}
