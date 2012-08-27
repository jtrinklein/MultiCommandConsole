using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiCommandConsole.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			Config.CommandPromptText = "console";
			Config.ShowConsoleCommand = true;
			Config.ShowVierArgsCommand = true;
			var engine = new Engine(new[] {typeof (Program).Assembly})
			             	{
			             		AppName="example_console",
								HistorySize = 5
			             	};
			engine.Run(args);
		}
	}

	[ConsoleCommand("repeat", "repeats the entered phrase the specified number of times")]
	public class RepeatConsoleCommand : IConsoleCommand
	{
		[Arg("text|t", "the text to be repeated", Required = true)]
		public string Text { get; set; }

		[Arg("times|n", "the number of times to repeat the text.  between 1 and 100")]
		public int Times { get; set; }

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
			for (int i = 0; i < Times; i++)
			{
				Console.Out.WriteLine(Text);
			}
		}
	}
}
