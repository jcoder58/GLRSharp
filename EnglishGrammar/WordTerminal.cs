using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar.String;
using GLR;
using GLR.Grammar;

namespace EnglishGrammar {
    internal class WordTerminal : Terminal {
         public override bool IsEpsilon { get { return false; } }
        public string MatchText { get; private set; }
        public override bool IsEndOfString {
            get { return false; }
        }

        public WordTerminal(string matchText) {
            MatchText = matchText;
        }

        public override Match<string> Match(ISource<string> source) {
            Match<string> match = new Match<string>(source,source.Offset, -1, this);
            var sourceString = source as Source;
            var endOfMatch = sourceString.Offset + MatchText.Length;
            if (sourceString != null && 
                endOfMatch <= sourceString.SourceText.Length &&
                sourceString.SourceText.IndexOf(MatchText, sourceString.Offset, MatchText.Length) == sourceString.Offset &&
                (endOfMatch == sourceString.SourceText.Length ||
                !char.IsLetter(sourceString.SourceText[endOfMatch]))
                ) {
                    match.Length = MatchText.Length;
            }
            return match;
        }

        public static ProductionList<string> operator |(WordTerminal a, WordTerminal b) {
            return new ProductionList<string>() { a, b };
        }


        public override string ToString() {
            return string.Format("'{0}'", MatchText);
        }

        public override object Value {
            get {
                return MatchText;
            }
            set {
                throw new NotImplementedException();
            }
        }
    
    }
}
