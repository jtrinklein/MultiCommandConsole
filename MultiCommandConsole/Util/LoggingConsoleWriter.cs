using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using ObjectPrinter;

namespace MultiCommandConsole.Util
{
    public class LoggingConsoleWriter : IConsoleWriter
    {
        private readonly ILog Log;

        public LoggingConsoleWriter(Type type)
        {
            Log = LogManager.GetLogger(type);
        }

        public void WriteErrorLine(object obj)
        {
            Log.Error(obj);
        }

        public void WriteErrorLine(string format, params object[] args)
        {
            Log.ErrorFormat(format, args);
        }

        public void WriteLine(object obj)
        {
            Log.Info(obj);
        }

        public void WriteLine(string format, params object[] args)
        {
            Log.InfoFormat(format, args);
        }

        public void WriteLines(params string[] lines)
        {
            Log.Info(lines.Dump());
        }

        public void WriteTable(string[] headers, IEnumerable<string[]> rows, TableFormat tableFormat = null)
        {
            var lines = Enumerable.Empty<string>();
            if (headers != null)
            {
                lines = lines.Union(headers);
            }
            if (rows != null)
            {
                lines = lines.Union(rows.Select(cells => string.Join(" - ", cells)));
            }

            Log.Info(lines.Dump());
        }
    }
}