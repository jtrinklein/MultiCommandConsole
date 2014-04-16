using System.Collections.Generic;

namespace MultiCommandConsole.Util
{
    public interface IConsoleWriter
    {
        void WriteErrorLine(object obj);
        void WriteErrorLine(string format, params object[] args);
        void WriteLine(object obj = null);
        void WriteLine(string format, params object[] args);
        void WriteLines(params string[] lines);
        void WriteTable(string[] headers, IEnumerable<string[]> rows, TableFormat tableFormat = null);
    }
}