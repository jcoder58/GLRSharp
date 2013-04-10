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
            TestStringGrammar();
        }

#pragma warning disable 1718
        private static void TestStringGrammar() {
            var a = Terminal.T("a");
            NonTerminal A = new NonTerminal("A");
            A.RHS = a;
            A.RHS = A < A;

            Debug.Assert(a.ToString() == "'a'");
            Debug.Assert(A.ToString() == "A");
            var text = A.RHS.ToString();

            Parser parser = new Parser( A, Log, LogLevel.Trace );
            parser.Log = Log;
            parser.Level = LogLevel.Debug;
        }

        static void Log(LogLevel level, string text) {
            //Console.WriteLine("{0}: {1}", level, text);

            Debug.WriteLine(text);
        }
    }
}
