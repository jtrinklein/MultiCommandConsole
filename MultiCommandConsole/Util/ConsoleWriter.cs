using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiCommandConsole.Util
{
    ///<summary>writes to Console.Out and Console.Error</summary>
    public class ConsoleWriter : IConsoleWriter
    {
        /// <summary>
        /// Gets a console writer for the given type
        /// </summary>
        public static IConsoleWriter Get<T>()
        {
            return Config.GetConsoleWriterDelegate(typeof (T));
        }
        /// <summary>
        /// Gets a console writer for the given type
        /// </summary>
        public static IConsoleWriter Get(Type type)
        {
            return Config.GetConsoleWriterDelegate(type);
        }

        private readonly TextWriter _errorWriter;
        private readonly TextWriter _writer;
        private readonly Func<int> _getScreenWidth;

        /// <summary></summary>
        public ConsoleWriter()
            : this(Console.Out, Console.Error, () => Math.Min(Console.BufferWidth, Console.WindowWidth))
        {
        }

        /// <summary></summary>
        public ConsoleWriter(TextWriter writer, TextWriter errorWriter, Func<int> getScreenWidth)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (errorWriter == null) throw new ArgumentNullException("errorWriter");
            _writer = writer;
            _errorWriter = errorWriter;
            _getScreenWidth = getScreenWidth;
        }

        /// <summary></summary>
        public void WriteErrorLine(object obj)
        {
            if (obj == null)
            {
                return;
            }
            WriteErrorLine(obj.ToString());
        }

        /// <summary></summary>
        public void WriteErrorLine(string format, params object[] args)
        {
            WriteLine(_errorWriter, format, args);
        }

        /// <summary></summary>
        public void WriteLine(object obj = null)
        {
            if (obj == null)
            {
                _writer.WriteLine();
            }
            else
            {
                WriteLine(obj.ToString());
            }
        }

        /// <summary></summary>
        public void WriteLine(string format, params object[] args)
        {
            WriteLine(_writer, format, args);
        }

        private void WriteLine(TextWriter writer, string format, object[] args)
        {
            var text = args.IsNullOrEmpty() ? format : string.Format(format, args);
            var chunks = text.GetChunks(_getScreenWidth());
            foreach (var chunk in chunks)
            {
                writer.WriteLine(chunk);
            }
        }

        /// <summary></summary>
        public void WriteLines(params string[] lines)
        {
            if (lines.IsNullOrEmpty())
            {
                throw new ArgumentNullException("lines");
            }

            foreach (var line in lines)
            {
                WriteLine(line);
            }
        }

        /// <summary></summary>
        public void WriteTable(string[] headers, IEnumerable<string[]> rows, TableFormat tableFormat = null)
        {
            tableFormat = tableFormat == null
                              ? new TableFormat()
                              : tableFormat.Clone();

            var columnsToAutoAdjust = tableFormat.Widths
                                                 .Select((width, index) => width >= 0 ? -1 : index)
                                                 .Where(i => i >= 0)
                                                 .ToArray();

            //if we need to autoalign, make sure we only enumerate the rows once
            // in case it can't be enumerated a second time
            var safeRows = columnsToAutoAdjust.IsNullOrEmpty() ? rows : rows.ToList();

            tableFormat.Widths = FixWidths(safeRows, tableFormat, columnsToAutoAdjust);

            if (!headers.IsNullOrEmpty())
            {
                headers.PivotChunks(tableFormat)
                       .ForEach(_writer.WriteLine);
            }

            if (safeRows.IsNullOrEmpty())
            {
                return;
            }

            foreach (var row in safeRows)
            {
                if (row.IsNullOrEmpty())
                {
                    _writer.WriteLine();
                    continue;
                }
                row.PivotChunks(tableFormat)
                   .ForEach(_writer.WriteLine);
            }
        }

        private int[] FixWidths(IEnumerable<string[]> rows, TableFormat tableFormat, int[] columnsToAutoAdjust)
        {
            var widths = tableFormat.Widths;

            bool autoAlign = !columnsToAutoAdjust.IsNullOrEmpty();

            if (autoAlign)
            {
                var maxWidths = new int[widths.Length];
                Array.Copy(widths, maxWidths, widths.Length);
                foreach (var row in rows)
                {
                    foreach (var i in columnsToAutoAdjust)
                    {
                        maxWidths[i] = Math.Max((row.SafeFromIndex(i) ?? string.Empty).Length, maxWidths[i]);   
                    }
                }

                widths = maxWidths.ToArray();
            }

            var spacerWidth =
                new[]
                    {
                        tableFormat.Spacer4Header.Length, 
                        tableFormat.Spacer4FirstRow.Length,
                        tableFormat.Spacer4OtherRows.Length
                    }.Max();
            var screenWidth = _getScreenWidth() - tableFormat.Indent.Length - spacerWidth;

            var totalWidth = 0;
            for (int index = 0; index < widths.Length; index++)
            {
                var width = widths[index];
                if (totalWidth > screenWidth)
                {
                    widths[index] = 0;
                }
                else if (totalWidth + width > screenWidth)
                {
                    widths[index] = screenWidth - totalWidth;
                    totalWidth = screenWidth;
                }
                else
                {
                    totalWidth += width;
                }
            }
            return widths;
        }
    }
}