using System.Collections.Generic;

namespace MultiCommandConsole.Util
{
    /// <summary>
    /// Defines interface for communicating to the user via the console
    /// </summary>
    public interface IConsoleWriter
    {
        /// <summary>writes a line to the error stream</summary>
        void WriteErrorLine(object obj);
        /// <summary>writes a line to the error stream</summary>
        void WriteErrorLine(string format, params object[] args);
        /// <summary>writes a line to the standard stream</summary>
        void WriteLine(object obj = null);
        /// <summary>writes a line to the standard stream</summary>
        void WriteLine(string format, params object[] args);
        /// <summary>writes lines to the standard stream</summary>
        void WriteLines(params string[] lines);
        /// <summary>writes a table to the standard stream</summary>
        void WriteTable(string[] headers, IEnumerable<string[]> rows, TableFormat tableFormat = null);
    }
}