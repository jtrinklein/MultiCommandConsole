using System.Collections.Generic;
using System.Linq;
using log4net;

namespace MultiCommandConsole.Example
{
	[ConsoleCommand("testLogOptions||log", "test the LogOptions")]
	public class TestLogOptionsConsoleCommand : IConsoleCommand
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(TestLogOptionsConsoleCommand));

		public LogOptions LogOptions { get; set; }

		public IEnumerable<string> GetArgValidationErrors()
		{
			return Enumerable.Empty<string>();
		}

		public string GetDetailedHelp()
		{
			return string.Empty;
		}

		public List<string> ExtraArgs { get; set; }
		
		public void Run()
		{
			Log.Fatal("Fatal");
			Log.Error("Error");
			Log.Warn("Warn");
			Log.Info("Info");
			Log.Debug("Debug");
		}
	}
}