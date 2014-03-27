using System;
using System.Collections.Generic;
using System.Text;

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

	    public static IEnumerable<string> PivotChunks(this string[] cells, string spacerFirstLine, string spacerOtherLines, int[] widths)
        {
            var line = new StringBuilder();
            int chunkIndex = 0;
            string spacer = spacerFirstLine;

            while(true)
            {
                bool hasText = false;
                line.Length = 0;

                for (int i = 0; i < cells.Length; i++)
                {
                    var width = widths.SafeFromIndex(i);
                    var chunk = cells[i].GetChunk(width, chunkIndex);
                    if (i > 0)
                    {
                        line.Append(spacer);
                    }
                    if (!chunk.IsNullOrEmpty())
                    {
                        line.Append(chunk.PadRight(width));
                        hasText = true;
                    }
                    else
                    {
                        line.Append("".PadRight(width));
                    }
                }
                if (!hasText)
                {
                    yield break;
                }

                yield return line.ToString();
                chunkIndex++;
                spacer = spacerOtherLines;
            }
        }

        public static string GetChunk(this string text, int chunkSize, int chunkIndex)
        {
            var startIndex = chunkSize*chunkIndex;
            if (startIndex > text.Length)
            {
                return null;
            }
            return text.Substring(startIndex, Math.Min(chunkSize, text.Length - startIndex));
        }

	    public static IEnumerable<string> Chunk(this string text, int chunkSize)
        {
            if (chunkSize > text.Length)
            {
                foreach (var chunk in text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return chunk;
                }
            }
            else
            {
                var startIndex = 0;
                while (startIndex < text.Length)
                {
                    var effectiveChunkSize = chunkSize;
                    var chunk = text.Substring(startIndex, Math.Min(chunkSize, text.Length - startIndex));
                    var newlineIndex = chunk.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    if (newlineIndex != -1)
                    {
                        effectiveChunkSize = newlineIndex + Environment.NewLine.Length;
                        chunk = chunk.Substring(0, effectiveChunkSize).Trim(Environment.NewLine.ToCharArray());
                    }
                    else if (chunk.Length == chunkSize)
                    {
                        for (int i = chunk.Length - 1; i >= 0; i--)
                        {
                            var c = chunk[i];

                            if (char.IsWhiteSpace(c) || c == '-')
                            {
                                effectiveChunkSize = i + 1;
                                chunk = chunk.Substring(0, effectiveChunkSize).Trim();
                                break;
                            }
                        }
                    }
                    yield return chunk;
                    startIndex += effectiveChunkSize;
                }
            }
        }
	}
}