using System;
using System.Collections.Generic;
using System.Linq;
using ObjectPrinter;

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

	[ArgSet("logger", "manages verbosity level so commands don't have to")]
	public class Logger
	{
		[Arg("quiet|q", "when specified, only errors will be logged")]
		public bool Quiet { get; set; }
		[Arg("verbose|v", "when specified, everything is logged.  Verbose trumps quiet.")]
		public bool Verbose { get; set; }

		public void Error(object message)
		{
			Console.Error.WriteLine(message);
		}
		public void Info(object message)
		{
			if (Verbose || !Quiet)
			{
				Console.Out.WriteLine(message);
			}
		}
		public void Trace(object message)
		{
			if (Verbose)
			{
				Console.Out.WriteLine(message);
			}
		}
	}

	[ConsoleCommand("repeat", "repeats the entered phrase the specified number of times")]
	public class RepeatConsoleCommand : IConsoleCommand
	{
		[Arg("text|t", "the text to be repeated", Required = true)]
		public string Text { get; set; }

		[Arg("times|n", "the number of times to repeat the text.  between 1 and 100")]
		public int Times { get; set; }

		public Logger Logger { get; set; }

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
			Logger.Trace(this.DumpToLazyString());
			for (int i = 0; i < Times; i++)
			{
				Console.Out.WriteLine(Text);
			}
		}
	}
}
