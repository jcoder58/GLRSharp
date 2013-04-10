using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR {
    public interface ISource<T> {
        int Offset { get; }
        bool AtEnd { get; }
        ISource<T> Move(int length);
        ISource<T> MoveTo(int offset);
        ISource<T> Skip(Func<T, int, int> Skip);

        string ToString(int start, int length);
    }
}
