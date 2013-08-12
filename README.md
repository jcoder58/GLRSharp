GLRSharp
========

GLR Combinatorial Parser written in C# 4.0.  


Introduction
------------

GLRSharp is a General LR parser implemented as a set of combinatorial functions in C#.  It currently only parses sources of type string, however it is architected using generic classes so parsing general binary sources can also be implemented.

GLRSharp implements a simple DSL embedded in C#.  The following examples shows most of the features of the DSL.  The example is based on the “Groucho Grammar” from Chapter 8 of Natural Language Processing with Python.  The full source code can be found in the GLRTest project

```C#
// Pre-declare the NonTerminals.  
NonTerminal S = new NonTerminal("S");
NonTerminal PP = new NonTerminal("PP");
NonTerminal NP = new NonTerminal("NP");
NonTerminal VP = new NonTerminal("VP");
NonTerminal Det = new NonTerminal("Det");
NonTerminal N = new NonTerminal("N");
NonTerminal V = new NonTerminal("V");
NonTerminal P = new NonTerminal("P");

// The operator < (or >) implements a sequence of symbols.
// The operator | implements alternative productions
// The extension method T() returns a string terminal matcher.
// To declare an action to be executed when a production is match, use the syntax
//  (P) [a => action(a)]

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
           

// Create a parser with the grammar, with an option Log function
Parser parser = new Parser(S, Log, LogLevel.Trace);

// Set the skip function to skip white space
parser.Skip = (source, offset) => {
    while (offset < source.Length && char.IsWhiteSpace(source[offset]))
        offset++;
    return offset;
};
parser.Log = Log;
parser.Level = LogLevel.Trace;

// Parse a test string.  This should return two parse trees
var results = parser.Parse("I shot an elephant in my pajamas");
var matches = parser.Matches;

```


