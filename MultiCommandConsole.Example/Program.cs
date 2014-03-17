using System.IO;
using System.Reflection;
using Common.Logging;

namespace MultiCommandConsole.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var assembly = typeof (Program).Assembly;
            log4net.Config.XmlConfigurator.ConfigureAndWatch(GetLogFileInfo(assembly));
            Config.ConsoleMode.Enabled = true;
			Config.ConsoleMode.CommandPromptText = "console";
		    Config.ConsoleMode.HistorySize = 5;
		    Config.ConsoleMode.AppName = "example_console";
			Config.ShowVierArgsCommand = true;
		    Config.DefaultCommand = typeof (RepeatConsoleCommand);

		    LogManager.Adapter = new Log4NetFactoryAdapter();

			var engine = new Engine(new[] {assembly});
			engine.Run(args);
		}

		private static FileInfo GetLogFileInfo(Assembly assembly)
		{
			if(File.Exists("log4net.config"))
			{
				return new FileInfo("log4net.config");
			}

			var appConfig = assembly.Location + ".config";
			if (File.Exists(appConfig))
			{
				return new FileInfo(appConfig);
			}
			throw new FileNotFoundException("Unable to locate log4net configs at log4net.config or " + appConfig);
		}
	}
}
