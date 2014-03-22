using System;
using System.Diagnostics;
using Common.Logging;

namespace MultiCommandConsole.Example
{
	class Program
	{
		static void Main(string[] args)
		{
            if (Array.Exists(args, s => s == "/debug") && Environment.UserInteractive)
            {
                Debugger.Break();
            }

            Config.ConsoleMode.Enabled = true;
			Config.ConsoleMode.CommandPromptText = "console";
		    Config.ConsoleMode.HistorySize = 5;
		    Config.ConsoleMode.AppName = "example_console";
			Config.ShowVierArgsCommand = true;
		    Config.DefaultCommand = typeof (RepeatConsoleCommand);

		    LogManager.Adapter = Log4NetFactoryAdapter.Load();

            var assembly = typeof(Program).Assembly;
		    new Engine(new[] {assembly}).GetRunner().Run(args);
		}
	}
}
