using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;

namespace MultiCommandConsole.Util
{
	public class ConsoleFormatter
	{
		private static readonly ILog Log = LogManager.GetLogger<ConsoleFormatter>();

		public int OverriddenBufferWidth { get; set; }

		private int BufferWidth { get { return OverriddenBufferWidth == 0 ? GetBufferWidth() : OverriddenBufferWidth; } }

		private int GetBufferWidth()
		{
			try
			{
				return Environment.UserInteractive ? Console.BufferWidth : 0;
			}
			catch
			{
				/* assume we're in a test */
				OverriddenBufferWidth = 80; //Windows default buffer size
				Log.Debug("buffer width not found.  is this a test?  assigning default of " + OverriddenBufferWidth);
				return OverriddenBufferWidth;
			}
		}

		public IEnumerable<string> ChunkString(string text, int decreaseChunkBy = 0)
		{
			var chunkSize = BufferWidth - decreaseChunkBy;

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

		public void ChunckStringTo(string text, TextWriter textWriter, string chunkPrefix = null)
		{
			chunkPrefix = chunkPrefix ?? string.Empty;

			foreach (var chunk in ChunkString(text, chunkPrefix.Length))
			{
				textWriter.WriteLine(chunkPrefix + chunk);
			}
		}

		public void ChunckStringTo(string text, Action<string> action, string chunkPrefix = null)
		{
			chunkPrefix = chunkPrefix ?? string.Empty;

			foreach (var chunk in ChunkString(text, chunkPrefix.Length))
			{
				action(chunkPrefix + chunk);
			}
		}
	}
}