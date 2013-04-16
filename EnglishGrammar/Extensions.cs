using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLR.Grammar;

namespace EnglishGrammar {
    internal static class Extensions {
        public static WordTerminal W(this string word) {
            return new WordTerminal(word);
        }

        public static ProductionList<string> P(this string wordList) {
            var fields = wordList.Split(',');
            ProductionList<string> list = new ProductionList<string>();
            foreach (var field in fields)
                list.Add(new WordTerminal(field.Trim().ToLowerInvariant()));
            return list;
        }
    }
}
