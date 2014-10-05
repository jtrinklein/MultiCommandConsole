using System;
using System.Collections.Generic;

namespace MultiCommandConsole.Util
{
    ///<summary>delegates methods to all provided console writers</summary>
    public class DelegatedConsoleWriter : IConsoleWriter
    {
        private readonly IConsoleWriter[] _innerWriters;

        ///<summary>ctors with the console writers to delegate to</summary>
        public DelegatedConsoleWriter(params IConsoleWriter[] innerWriters)
        {
            if (innerWriters == null) throw new ArgumentNullException("innerWriters");
            _innerWriters = innerWriters;
        }

        public void WriteErrorLine(object obj)
        {
            _innerWriters.ForEach(w => w.WriteErrorLine(obj));
        }

        public void WriteErrorLine(string format, params object[] args)
        {
            _innerWriters.ForEach(w => w.WriteErrorLine(format, args));
        }

        public void WriteLine(object obj = null)
        {
            _innerWriters.ForEach(w => w.WriteLine(obj));
        }

        public void WriteLine(string format, params object[] args)
        {
            _innerWriters.ForEach(w => w.WriteLine(format, args));
        }

        public void WriteLines(params string[] lines)
        {
            _innerWriters.ForEach(w => w.WriteLines(lines));
        }

        public void WriteTable(string[] headers, IEnumerable<string[]> rows, TableFormat tableFormat = null)
        {
            _innerWriters.ForEach(w => w.WriteTable(headers, rows, tableFormat));
        }
    }
}