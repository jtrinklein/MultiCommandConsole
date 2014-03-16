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

		public static string[] SplitCmdLineArgs(this string consoleInput)
		{
		    return new CommandLineParser().Parse(consoleInput);
		}
	}
}