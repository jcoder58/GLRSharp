using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar {
    public interface ISymbol<T> {
        bool IsNullable { get; }
        bool IsNonTerminal { get; }
        bool IsTerminal { get; }
        bool IsEpsilon { get; }
        bool IsEndOfString { get; }
        Match<T> Match(ISource<T> source);

        void Visit( Action<ISymbol<T>> action, Set<ISymbol<T>> visited);

        Set<Terminal<T>> Terminals { get; }
        Set<NonTerminal<T>> NonTerminals { get; }


        object Value { get; set; }
    }
}
