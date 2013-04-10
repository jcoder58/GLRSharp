using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR {
    internal static class Extensions {
        public static HashSet<T> ToSet<T>(this IEnumerable<T> collection) {
            return new HashSet<T>(collection);
        }
    }
}


