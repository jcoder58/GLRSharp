using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR {
    public interface ISource<T> {
        int Offset { get; }
        bool AtEnd { get; }
        Source Move(int length);
        Source MoveTo(int offset);
        Source Skip(Func<T, int, int> Skip);

        string ToString(int start, int length);
    }
}
