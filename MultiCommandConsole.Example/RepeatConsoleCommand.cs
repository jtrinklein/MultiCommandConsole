using System;
using System.Collections.Generic;
using ObjectPrinter;
using Common.Logging;

namespace MultiCommandConsole.Example
{
	[ConsoleCommand("repeat", "repeats the entered phrase the specified number of times")]
	public class RepeatConsoleCommand : IConsoleCommand
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (RepeatConsoleCommand));

		[Arg("text|t", "the text to be repeated", Required = true)]
		public string Text { get; set; }

		[Arg("times|n", "the number of times to repeat the text.  between 1 and 100")]
		public int Times { get; set; }

		public LogOptions LogOptions { get; set; }

		public IEnumerable<string> GetArgValidationErrors()
		{
			var errors = new List<string>();
			if (Times < 1)
			{
				errors.Add("times must be greater than 0");
			}
			if (Times > 100)
			{
				errors.Add("times must be less than 101");
			}
			return errors;
		}

		public string GetDetailedHelp()
		{
			return string.Empty;
		}

		public List<string> ExtraArgs { get; set; }

		public void Run()
		{
			Log.Debug(this.Dump());

			for (int i = 0; i < Times; i++)
			{
				Console.Out.WriteLine(Text);
			}
		}
	}
}