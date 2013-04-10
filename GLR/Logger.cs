using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR {
    class Logger {
        public Action<LogLevel, string> Log { get; set; }
        public Func<LogLevel> Level { get; set; }

        public bool Debug { get { return Test(LogLevel.Debug); } }
        public bool Trace { get { return Test(LogLevel.Trace); } }

        public Logger(Func<LogLevel> level, Action<LogLevel, string> log) {
            Log = log;
            Level = level;
        }

        public bool Test(LogLevel match) {
            return Level() <= match;
        }

        internal void LogTrace(string format, params object[] args) {
            if (Trace)
                Log(LogLevel.Trace, string.Format(format, args));
        }

        internal void LogDebug(string format, params object[] args) {
            if (Debug)
                Log(LogLevel.Debug, string.Format(format, args));
        }
 }
}
