using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public class Source : ISource<string> {
        public string SourceText { get; private set; }
        public int Offset { get; private set; }

        public bool AtEnd { get { return Offset >= SourceText.Length; } }

        public Source(string sourceText, int offset) {
            SourceText = sourceText;
            Offset = offset;
        }

        public ISource<string> Move(int length) {
            return new Source(SourceText, Offset + length);
        }

        public ISource<string> MoveTo(int offset) {
            return new Source(SourceText, offset);
        }

        public ISource<string> Skip(Func<string, int, int> Skip) {
            int newOffset = Skip(SourceText, Offset);
            if (newOffset != Offset)
                return new Source(SourceText, newOffset);
            return this;
        }
        
        public override bool Equals(object obj) {
            if (obj is Source) {
                var s = obj as Source;
                return SourceText.Equals(s.SourceText) && Offset == s.Offset;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return SourceText.GetHashCode() ^ Offset.GetHashCode();
        }

        public override string ToString() {
            char bullet = '•';
            if (Offset == 0)
                return bullet + SourceText;
            if (Offset == SourceText.Length)
                return SourceText + bullet;
            return System.String.Format("{0}{1}{2}", SourceText.Substring(0, Offset), bullet, SourceText.Substring(Offset));
        }


        public string ToString(int start, int length) {
            return SourceText.Substring(start, length);
        }
    }
}
