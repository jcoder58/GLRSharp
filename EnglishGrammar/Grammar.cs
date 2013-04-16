using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar.String;

namespace EnglishGrammar {
    public class Grammar {
        //https://docs.google.com/document/d/1sYnGJAsLvmRaqrBP9xzMGQetVxtaHzyKvUfTQYHmcKA/edit
        public NonTerminal Root { get; private set; }

        public Grammar() {
            var dialog = new NonTerminal("Dialog");
            var question = new NonTerminal("Question");
            var sentence = new NonTerminal("Sentence");
            var sentences = new NonTerminal("Sentences");
            var subject = new NonTerminal("Subject");
            var predicate = new NonTerminal("Predicate");
            var simpleSubject = new NonTerminal("SimpleSubject");
            var compoundSubject = new NonTerminal("CompoundSubject");
            var nounPhrase = new NonTerminal("NounPhrase");

            var tobeVerb = new NonTerminal("ToBeVerb");
            var verb = new NonTerminal("Verb");
            var noun = new NonTerminal("Noun");
            var pronoun = new NonTerminal("Pronoun");
            var preposition = new NonTerminal("Preposition");
            var interrogative = new NonTerminal("Interrogative");
            var conjunction = new NonTerminal("conjunction");
            var subordinateConjunction = new NonTerminal("SubordinateConjunction");
            var auxillaryVerb = new NonTerminal("AuxillaryVerb");
            var article = new NonTerminal("Article");
            var adverb = new NonTerminal("Adverb");
            var adjective = new NonTerminal("Adjective");

            dialog.RHS =    question | 
                            sentences;

            sentences.RHS = sentences < sentence |
                            sentence;

            sentence.RHS =  subject < predicate;

            subject.RHS =   simpleSubject | 
                            compoundSubject;

            simpleSubject.RHS = nounPhrase | "it".W() | "they".W();

            tobeVerb.RHS = "be,being,been,am,is,are,was,were".P();

            verb.RHS =      tobeVerb | 
                            "run".W();

            preposition.RHS = "from, toward, in, about, over, above, under, at, below, off".P();

            interrogative.RHS = "who, what, where, when, why, how, whose, whom".P();

            pronoun.RHS =   "this, that, such".P() |
                            "who, which".P() |
                            "each, either, some, any, many, few, all".P() ;

            conjunction.RHS = "and, or, but".P();

            subordinateConjunction.RHS = "where, when, while, because, if, unless".P();

            auxillaryVerb.RHS = "will, would, may, might, shall, should, can, could, must".P();

            article.RHS = "a, an, the".P();

            adjective.RHS = "new,newer,newest".P();

            adverb.RHS = "very".P();
        }
    }
}
