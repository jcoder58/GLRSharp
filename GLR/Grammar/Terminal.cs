using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar {
    public abstract class Terminal<T> : ISymbol<T> {
        public readonly static Match<T> Failed = new Match<T>() { Length = -1, Source = null, Start = -1 };

        public bool IsNullable { get { return this is E<T> ? true : false; } }
        public bool IsNonTerminal { get { return false; } }
        public bool IsTerminal { get { return !IsNonTerminal; } }

        public abstract bool IsEpsilon { get; }
        public abstract bool IsEndOfString { get; }

        public abstract Match<T> Match(ISource<T> source);

        Set<Terminal<T>> _Terminals = null;
        public Set<Terminal<T>> Terminals {
            get {
                if (_Terminals == null) {
                    _Terminals = new Set<Terminal<T>>() { this };
                }
                return _Terminals;
            }
        }

        static Set<NonTerminal<T>> _NonTerminals = new Set<NonTerminal<T>>();
        public Set<NonTerminal<T>> NonTerminals {
            get { return _NonTerminals; }
        }

        public void Visit(Action<ISymbol<T>> action, Set<ISymbol<T>> visited) {
            if (visited != null)
                visited.Add(this);
            action(this);
        }
    }
}
