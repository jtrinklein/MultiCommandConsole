using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectPrinter;

namespace MultiCommandConsole.Util
{
	public static class StringExtensions
	{
		public static string[] GetPrototypeArray(this string prototype)
		{
			return prototype.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string[] SplitCmdLineArgs(this string consoleInput)
		{
		    return new CommandLineParser().Parse(consoleInput);
		}

        public static IEnumerable<string> PivotChunks(this string[] cells, string spacer, int[] widths)
        {
            return PivotChunks(cells, spacer, spacer, widths);
        }

        public static IEnumerable<string> PivotChunks(this string[] cells, string spacer4FirstRow, string spacer4OtherRows, int[] widths)
        {
            return PivotChunks(cells, new TableFormat
                {
                    Spacer4FirstRow = spacer4FirstRow,
                    Spacer4OtherRows = spacer4OtherRows,
                    Widths = widths
                });
        }

	    public static IEnumerable<string> PivotChunks(this string[] cells, TableFormat tableFormat)
        {
            if (cells.IsNullOrEmpty())
            {
                yield break;
            }

	        var widths = tableFormat.Widths;
            var spacer = tableFormat.Spacer4FirstRow;

            //break cells into lines
	        var cellsWithLines = new List<string>[cells.Length];
	        var maxLineCount = 0;
	        for (int i = 0; i < cells.Length; i++)
            {
                var width = widths.SafeFromIndex(i);
                var lines = cells[i].GetChunks(width).ToList();
                cellsWithLines[i] = lines;
                maxLineCount = Math.Max(maxLineCount, lines.Count);
            }

            //construct each line
            var line = new StringBuilder();
	        for (int l = 0; l < maxLineCount; l++)
	        {
	            line.Append(tableFormat.Indent);
	            for (int c = 0; c < cellsWithLines.Length; c++)
                {
                    var cell = cellsWithLines[c];
                    var cellLine = cell.SafeFromIndex(l);
                    var width = widths.SafeFromIndex(c);
                    if (c > 0)
                    {
                        line.Append(spacer);
                    }
                    line.Append(cellLine == null 
                        ? "".PadRight(width) 
                        : cellLine.PadRight(width));
                }
	            yield return line.ToString();
                line.Length = 0;
                spacer = tableFormat.Spacer4OtherRows;
	        }
        }

	    public static IEnumerable<string> GetChunks(this string text, int chunkSize)
        {
            if (text == null)
            {
                yield break;
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                yield return text;
                yield break;
            }

	        var lines = text.Split(new[] {Environment.NewLine, "\n", "\r"}, StringSplitOptions.None);

	        foreach (var line in lines)
	        {
	            if (string.IsNullOrEmpty(line))
	            {
	                yield return string.Empty;
	            }
                else if (chunkSize > line.Length)
                {
                    yield return line.TrimEnd();
                }
                else
                {
                    var startIndex = 0;
                    while (startIndex < line.Length)
                    {
                        //add one to chunksize to include viable line breaks when splitting words
                        var chunk = line.WholeWordSubstring(startIndex, chunkSize, out startIndex);
                        yield return chunk.Trim();
                        startIndex++;
                    }
                }
	        }
        }

        internal static string WholeWordSubstring(this string text, int startIndex, int length, out int endIndex)
        {
            int start = startIndex;
            char currentChar = '\0';
            var nextChar = '\0';
            var currentIndex = start;
            var lastLineBreak = 0;
            var lastPunctuation = 0;
            var endOfString = false;

            try
            {
                while (start < text.Length && char.IsWhiteSpace(text[start]))
                {
                    start++;
                }
                if (start == text.Length)
                {
                    //remaining string is all whitepaces
                    endIndex = text.Length - 1;
                    return null;
                }

                while (true)
                {
                    currentChar = text[currentIndex];

                    if (char.IsWhiteSpace(currentChar))
                    {
                        lastLineBreak = currentIndex;
                    }
                    else if (char.IsPunctuation(currentChar))
                    {
                        lastPunctuation = currentIndex;
                    }

                    if (currentIndex == text.Length - 1)
                    {
                        endOfString = true;
                        break;
                    }

                    if ((currentIndex - start) == length - 1)
                    {
                        nextChar = text[currentIndex + 1];
                        break;
                    }

                    currentIndex++;
                }

                if (endOfString 
                    || currentIndex == lastLineBreak || currentIndex == lastPunctuation 
                    || char.IsWhiteSpace(nextChar) || char.IsPunctuation(nextChar))
                {
                    endIndex = currentIndex;
                    return text.Substring(start, endIndex - start + 1);
                }

                var lastBreak = Math.Max(lastLineBreak, lastPunctuation);
                if (lastBreak > 0 && lastBreak == lastLineBreak)
                {
                    endIndex = lastBreak;
                    return text.Substring(start, endIndex - start);
                }
                if (lastBreak > 0 && lastBreak == lastPunctuation)
                {
                    endIndex = lastPunctuation;
                    return text.Substring(start, endIndex - start + 1);
                }

                endIndex = currentIndex;
                return text.Substring(start, endIndex - start + 1);
            }
            catch (Exception e)
            {
                e.SetContext("text", text);
                e.SetContext("startIndex", startIndex);
                e.SetContext("start", start);
                e.SetContext("currentIndex", currentIndex);
                e.SetContext("currentChar", currentChar);
                e.SetContext("nextChar", nextChar);
                e.SetContext("lastLineBreak", lastLineBreak);
                e.SetContext("lastPunctuation", lastPunctuation);
                throw;
            }
        }
	}
}