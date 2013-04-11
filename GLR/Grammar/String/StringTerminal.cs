using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public class StringTerminal : Terminal {
        public override bool IsEpsilon { get { return false; } }
        public string MatchText { get; private set; }
        public override bool IsEndOfString {
            get { return false; }
        }

        public StringTerminal(string matchText) {
            MatchText = matchText;
        }

        public override Match<string> Match(ISource<string> source) {
            Match<string> match = new Match<string>() { Source = source, Start = source.Offset, Terminal = this, Length = -1 };
            var sourceString = source as Source;
            if (sourceString != null && sourceString.Offset + MatchText.Length <= sourceString.SourceText.Length) {
                if (sourceString.SourceText.IndexOf(MatchText, sourceString.Offset, MatchText.Length) == sourceString.Offset)
                    match.Length = MatchText.Length;
            }
            return match;
        }

        public override string ToString() {
            return string.Format("'{0}'", MatchText);
        }
    }
}
