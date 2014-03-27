using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiCommandConsole.Util
{
    public class ConsoleWriter : IConsoleWriter
    {
        //TODO: use Chunker to chunck string to best fit the current console window

        private readonly TextWriter _writer;
        private readonly Func<int> _getScreenWidth;

        public ConsoleWriter()
            : this(Console.Out, () => Console.BufferWidth)
        {
        }

        public ConsoleWriter(TextWriter writer, Func<int> getScreenWidth)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            _writer = writer;
            _getScreenWidth = getScreenWidth;
        }

        public void Write(string format, params object[] args)
        {
            _writer.Write(format, args);
        }
        public void WriteLine(string format, params object[] args)
        {
            Write(format, args);
            _writer.WriteLine();
        }
        public void WriteLines(params string[] lines)
        {
            if (lines.IsNullOrEmpty())
            {
                throw new ArgumentNullException("lines");
            }

            foreach (var line in lines)
            {
                _writer.WriteLine(line);
            }
        }

        public void WriteTable(string[] headers, IEnumerable<string[]> rows, TableFormat tableFormat = null)
        {
            tableFormat = tableFormat ?? new TableFormat
                {
                    Spacer4Header = "  ",
                    Spacer4FirstRow = ": ",
                    Spacer4OtherRows = "  "
                };

            //if we need to autoalign, make sure we only enumerate the rows once
            // in case it can't be enumerated a second time
            var safeRows = tableFormat.AutoAlignWidth ? rows.ToList() : rows;

            var widths = FixWidths(safeRows, tableFormat.Widths, tableFormat.AutoAlignWidth);

            if (!headers.IsNullOrEmpty())
            {
                headers.PivotChunks(tableFormat.Spacer4Header, widths)
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
                row.PivotChunks(tableFormat.Spacer4Header, tableFormat.Spacer4OtherRows, widths)
                   .ForEach(_writer.WriteLine);
            }
        }

        private int[] FixWidths(IEnumerable<string[]> rows, int[] widths, bool autoAlign)
        {
            if (autoAlign)
            {
                var maxWidths = new List<int>();
                foreach (var row in rows)
                {
                    for (var i = 0; i < row.Length; i++)
                    {
                        maxWidths[i] = Math.Max((row.SafeFromIndex(i) ?? string.Empty).Length, maxWidths[i]);
                    }
                }

                widths = maxWidths.ToArray();
            }
            else
            {
                widths = widths ?? new int[0];
            }

            var screenWidth = _getScreenWidth();

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