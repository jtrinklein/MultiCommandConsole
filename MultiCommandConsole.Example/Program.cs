using System;
using System.Diagnostics;
using System.Reflection;
using Common.Logging;
using MultiCommandConsole.Services;
using ObjectPrinter;

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

		    try
		    {
		        new Engine(new[]
		            {
		                typeof(Program).Assembly, 
		                typeof(InstallServiceCommand).Assembly
		            }).GetRunner().Run(args);
		    }
		    catch (Exception e)
		    {
		        Console.Out.WriteLine(e.Dump());
		    }
		}
	}
}
