using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public class StringTerminal : Terminal {
        public override bool IsEpsilon { get { return false; } }
        public string MatchText { get; private set; }

        public StringTerminal(string matchText) {
            MatchText = matchText;
        }

        public override Match<string> Match(ISource<string> source) {
            Match<string> match = new Match<string>() { Source = source, Start = source.Offset, Terminal = this, Length = -1 };
            if (source is Source) {
                var s = source as Source;
                if (s.SourceText.IndexOf(MatchText, s.Offset, MatchText.Length) == s.Offset)
                    match.Length = MatchText.Length;
            }
            return match;
        }

        public override string ToString() {
            return string.Format("'{0}'", MatchText);
        }
    }
}
