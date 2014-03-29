using System.Collections.Generic;

namespace MultiCommandConsole.Util
{
    public interface IConsoleWriter
    {
        void WriteLine(object obj);
        void WriteLine(string format, params object[] args);
        void WriteLines(params string[] lines);
        void WriteTable(string[] headers, IEnumerable<string[]> rows, TableFormat tableFormat = null);
    }
}