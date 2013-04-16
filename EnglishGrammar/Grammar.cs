using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar.String;

namespace EnglishGrammar {
    public class Grammar {
        public NonTerminal Root { get; private set; }

        public Grammar() {
            NonTerminal dialog = new NonTerminal("Dialog");
            NonTerminal question = new NonTerminal("Question");
            NonTerminal sentence = new NonTerminal("Sentence");
            NonTerminal sentences = new NonTerminal("Sentences");
            NonTerminal subject = new NonTerminal("Subject");
            NonTerminal predicate = new NonTerminal("Predicate");

            dialog.RHS = question | sentences;

            sentences.RHS = sentences < sentence |
                            sentence;

            sentence.RHS = subject < predicate;

            subject.RHS = "it".W() | "they".W();
        }
    }
}
