using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GLR.Grammar {
    public class NonTerminal<T> : ISymbol<T> {

        ProductionList<T> _Productions = new ProductionList<T>();

        public ProductionList<T> RHS {
            get {
                return this._Productions;
            }
            set {
                foreach (var p in value)
                    p.LHS = this;
                this._Productions.AddRange(value);
            }
        }
        public bool IsEpsilon { get { return false; } }
        public bool IsEndOfString { get { return false; } }
        public bool IsNonTerminal { get { return true; } }
        public bool IsTerminal { get { return !IsNonTerminal; } }

        public string Name { get; private set; }
        public bool IsGrammarRoot { get; set; }

        public NonTerminal(string name = "") {
            Name = name;
            IsGrammarRoot = false;
        }

        bool? _IsNullable = null;

        public bool IsNullable {
            get {
                if (_IsNullable == null) {
                    _IsNullable = false;
                    var isnullable = RHS.Any(
                        production => ((IList<ISymbol<T>>)production).All(symbol => symbol.IsNullable)
                        );

#if DEBUG
                    foreach (var production in _Productions) {
                        bool isNullable = true;
                        foreach (var symbol in production) {
                            if (!symbol.IsNullable) {
                                isNullable = false;
                                break;
                            }
                        }
                        if (isNullable) {
                            _IsNullable = true;
                            break;
                        }
                    }
                    Debug.Assert(_IsNullable.Value == isnullable);
#endif
                }

                return _IsNullable.Value;
            }
        }

        public Match<T> Match(ISource<T> source) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return Name != null ? Name + (IsNullable ? "*" : "") : "Unknown";
        }

        #region Operators
        public static Production<T> operator <(NonTerminal<T> a, NonTerminal<T> b) {
            return (Production<T>)a < (Production<T>)b;
        }

        public static Production<T> operator >(NonTerminal<T> a, NonTerminal<T> b) {
            return a < b;
        }

        public static Production<T> operator <(NonTerminal<T> a, Terminal<T> b) {
            return (Production<T>)a < (Production<T>)b;
        }

        public static Production<T> operator >(NonTerminal<T> a, Terminal<T> b) {
            return a < b;
        }

        public static Production<T> operator <(Terminal<T> a, NonTerminal<T> b) {
            return (Production<T>)a < (Production<T>)b;
        }

        public static Production<T> operator >(Terminal<T> a, NonTerminal<T> b) {
            return a < b;
        }

        public static ProductionList<T> operator |(NonTerminal<T> a, NonTerminal<T> b) {
            ProductionList<T> productions = new ProductionList<T>() { a, b };
            return productions;
        }

        public static ProductionList<T> operator |(NonTerminal<T> a, Terminal<T> b) {
            ProductionList<T> productions = new ProductionList<T>() { a, b };
            return productions;
        }

        #endregion


        Set<Terminal<T>> _Terminals = null;
        public Set<Terminal<T>> Terminals {
            get {
                if (_Terminals == null) {
                    _Terminals = new Set<Terminal<T>>();
                    foreach (var pl in RHS)
                        _Terminals.UnionWith(pl.Terminals);
                }
                return _Terminals;
            }
        }

        Set<NonTerminal<T>> _NonTerminals;
        public Set<NonTerminal<T>> NonTerminals {
            get {
                if (_NonTerminals == null) {
                    _NonTerminals = new Set<NonTerminal<T>>() { this };
                    foreach (var pl in RHS)
                        _NonTerminals.UnionWith(pl.NonTerminals);
                }
                return _NonTerminals;
            }
        }


        public void Visit(Action<ISymbol<T>> action, Set<ISymbol<T>> visited) {
            if (visited == null)
                visited = new Set<ISymbol<T>>();
            if (!visited.Contains(this)) {
                visited.Add(this);
                foreach (var production in _Productions)
                    production.Visit(action, visited);
                action(this);
            }
        }


        public object Value {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }
    }

}
