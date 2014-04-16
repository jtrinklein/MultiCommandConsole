using System.Collections.Generic;
using System.Linq;
 using MultiCommandConsole.Util;

namespace MultiCommandConsole.Example
{
	[ConsoleCommand("repro", "this command is used to repro whatever bug i'm currently working on")]
	public class ReproConsoleCommand : IConsoleCommand
    {
        internal IConsoleWriter Writer { get; set; }

		public LogOptions LogOptions { get; set; }

		[Arg("required", "this arg is required", Required = true)]
        public string RequiredArg { get; set; }

        [Arg("superComment", 
            "this arg has some detailed comments" +
            "\r NOTE:" +
            "\r - this arg has formatting", 
            Required = true)]
        public string SuperComments { get; set; }

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
			Writer.WriteLine("RequiredArg=" + RequiredArg);
		}
	}
}