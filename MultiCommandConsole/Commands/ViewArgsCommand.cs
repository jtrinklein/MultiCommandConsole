using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiCommandConsole.Commands
{
	[ConsoleCommand(CommandName, "experiment with how the command line splits your arguments")]
	internal class ViewArgsCommand : IConsoleCommand
	{
		public const string CommandName = "viewargparsing"; 
		public string GetDetailedHelp()
		{
			return
				"Use this to experiment with how to use double quotes to keep args with spaces "
				+ "from being split and how to escape double quotes."
				+ Environment.NewLine
				+ Environment.NewLine
				+ "try this: " + CommandName + " \"arg with spaces \\\"and with quotes\\\"\" \\\"args without spaces but with quotes\\\"";
		}

		public List<string> ExtraArgs { get; set; }

		public IEnumerable<string> GetArgValidationErrors()
		{
			return Enumerable.Empty<string>();
		}

		public void Run()
		{
			foreach (var arg in ExtraArgs)
			{
				Config.ConsoleWriter.WriteLine(arg);
			}
		}
	}
}