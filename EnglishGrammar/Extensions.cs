using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnglishGrammar {
    internal static class Extensions {
        public static WordTerminal W(this string word) {
            return new WordTerminal(word);
        }
    }
}
