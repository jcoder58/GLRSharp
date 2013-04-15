using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GLR.Grammar.String;
using GLR;
using System.Diagnostics;

namespace GLRTest {
    class Program {
        static void Main(string[] args) {
            TestGrammar();
        }

        private static void TestGrammar() {
            GrouchoGrammar();
            TestGLR();
            TestLALR2();
            TestStringGrammar();
        }

#pragma warning disable 1718
        private static void TestLALR2() {
            Log(LogLevel.Info, "Starting TestLALR2()");
            var G = new NonTerminal("G");
            var S = new NonTerminal("S");
            var E = new NonTerminal("E");
            var N = new NonTerminal("N");
            var V = new NonTerminal("V");

            S.RHS = N;
            N.RHS = V < "=".T() < E;
            N.RHS = E;
            E.RHS = V;
            V.RHS = "x".T();
            V.RHS = "*".T() < E;

            Log(LogLevel.Info, "End TestLALR2()");
            Parser parser = new Parser(S, Log, LogLevel.Trace);
            var ok = parser.Parse("x=*x");
            Debug.Assert(ok);
        }
        
        private static void TestStringGrammar() {
            Log(LogLevel.Info, "Starting TestStringGrammar()");
            var a = Terminal.T("a");
            NonTerminal A = new NonTerminal("A");
            A.RHS = a;
            A.RHS = A < A;

            Debug.Assert(a.ToString() == "'a'");
            Debug.Assert(A.ToString() == "A");
            var text = A.RHS.ToString();

            Parser parser = new Parser( A, Log, LogLevel.Trace );
            Log(LogLevel.Info, "End TestStringGrammar()");

            var ok = parser.Parse(new Source( "a", 0));
            Debug.Assert(ok);
        }

        private static void TestGLR() {
            Log(LogLevel.Info, "Starting TestLALR()");
            var S = new NonTerminal("S");
            var E = new NonTerminal("E");

            S.RHS = E;
            E.RHS = "i".T();
            E.RHS = E < "*".T() < E;
            E.RHS = E < "+".T() < E;

            Parser parser = new Parser(E, Log, LogLevel.Trace);
            var ok = parser.Parse("i+i*i");
            Debug.Assert(ok);
            Log(LogLevel.Info, "End TestLALR()");
        }

        /*
    ... S -> NP VP
    ... PP -> P NP
    ... NP -> Det N | Det N PP | 'I'
    ... VP -> V NP | VP PP
    ... Det -> 'an' | 'my'
    ... N -> 'elephant' | 'pajamas'
    ... V -> 'shot'
    ... P -> 'in'
 */
        static void GrouchoGrammar() {
            NonTerminal S = new NonTerminal("S");
            NonTerminal PP = new NonTerminal("PP");
            NonTerminal NP = new NonTerminal("NP");
            NonTerminal VP = new NonTerminal("VP");
            NonTerminal Det = new NonTerminal("Det");
            NonTerminal N = new NonTerminal("N");
            NonTerminal V = new NonTerminal("V");
            NonTerminal P = new NonTerminal("P");

            S.RHS = (NP < VP)[a => string.Format( "S({0},{1})", a[0], a[1])];
            PP.RHS = (P < NP)[a => string.Format("PP({0},{1})", a[0], a[1])];
            NP.RHS = (Det < N)[a => string.Format("NP({0},{1})", a[0], a[1])] |
                (Det < N < PP)[a => string.Format("NP({0},{1},{2})", a[0], a[1],a[2])] | 
                "I".T();
            VP.RHS = (V < NP)[a => string.Format( "VP({0},{1})", a[0], a[1])] |
                (VP < PP)[a => string.Format("VP({0},{1})", a[0], a[1])];
            Det.RHS = "an".T() | "my".T();
            N.RHS = "elephant".T() | "pajamas".T();
            V.RHS = "shot".T();
            P.RHS = "in".T();
           

            Parser parser = new Parser(S, Log, LogLevel.Trace);
            parser.Skip = (source, offset) => {
                while (offset < source.Length && char.IsWhiteSpace(source[offset]))
                    offset++;
                return offset;
            };
            parser.Log = Log;
            parser.Level = LogLevel.Trace;
            var results = parser.Parse("I shot an elephant in my pajamas");
            var matches = parser.Matches;
            //var ok = parser.Parse("I shot  my pajamas");
            //ok = parser.Parse("I shot an elephant");
            Debug.Assert(results);
        }



        static void Log(LogLevel level, string text) {
            //Console.WriteLine("{0}: {1}", level, text);

            Debug.WriteLine(text);
        }
    }
}
