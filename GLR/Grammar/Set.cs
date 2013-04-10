using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar {
    public class Set<T> : HashSet<T> {

        public override string ToString() {
            StringBuilder builder = new StringBuilder("{");
            foreach (var t in this)
                builder.AppendFormat("{0}{1}", builder.Length > 1 ? "," : "", t);
           
            return builder.Append('}').ToString();
        }
    }
}
