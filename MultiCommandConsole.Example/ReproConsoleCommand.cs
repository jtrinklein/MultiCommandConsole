using System.Collections.Generic;
using System.Linq;
 using MultiCommandConsole.Util;

namespace MultiCommandConsole.Example
{
	[ConsoleCommand("repro", "this command is used to repro whatever bug i'm currently working on")]
	public class ReproConsoleCommand : IConsoleCommand
    {
        private IConsoleWriter _writer;

		public LogOptions LogOptions { get; set; }

		[Arg("required", "this arg is required", Required = true)]
		public string RequiredArg { get; set; }

		public IEnumerable<string> GetArgValidationErrors()
		{
			return Enumerable.Empty<string>();
		}

		public string GetDetailedHelp()
		{
			return null;
		}

		public List<string> ExtraArgs { get; set; }

		public void Run()
		{
			_writer.WriteLine("RequiredArg=" + RequiredArg);
		}
	}
}