using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using ObjectPrinter;

namespace MultiCommandConsole.Util
{
    public class LoggingConsoleWriter : IConsoleWriter
    {
        private static readonly ILog Log = LogManager.GetLogger<LoggingConsoleWriter>();

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
            Log.Info(headers.Union(rows.Select(cells => string.Join(" - ", cells))).Dump());
        }
    }
}