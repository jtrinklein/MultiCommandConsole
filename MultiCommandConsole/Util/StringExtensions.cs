using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiCommandConsole.Util
{
	public static class StringExtensions
	{
		public static string[] GetPrototypeArray(this string prototype)
		{
			return prototype.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string EscapeDoubleQuotes(this string text)
		{
			return text.Replace("\"", "\\\"");
		}

		public static string[] SplitCmdLineArgs(this string consoleInput)
		{
			var args = new List<string>();

			const char escapeChar = '\\';
			const char doubleQuote = '"';
			bool prevCharIsEscapeChar = false;
			bool isInDoubleQuote = false;

			var arg = new StringBuilder();

			foreach (var c in consoleInput)
			{
				//escape char is only an escape char when it precedes a double quote
				switch (c)
				{
					case doubleQuote:
						if (prevCharIsEscapeChar)
						{
							arg.Append(doubleQuote);
						}
						else
						{
							isInDoubleQuote = !isInDoubleQuote;
						}
						break;
					case escapeChar:
						if(prevCharIsEscapeChar)
						{
							arg.Append(escapeChar);
						}

						prevCharIsEscapeChar = true;
						continue;
						
						break;
					default:
						if(prevCharIsEscapeChar)
						{
							arg.Append(escapeChar);
							arg.Append(c);
						}
						else if(isInDoubleQuote)
						{
							arg.Append(c);
						}
						else if(char.IsWhiteSpace(c))
						{
							args.Add(arg.ToString());
							arg.Length = 0;
						}
						else
						{
							arg.Append(c);
						}
						break;
				}
				prevCharIsEscapeChar = false;

			}

			args.Add(arg.ToString());
			return args.Where(a => !string.IsNullOrEmpty(a)).ToArray();
		}
	}
}