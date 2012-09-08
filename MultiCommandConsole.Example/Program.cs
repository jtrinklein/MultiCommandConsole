using System.IO;
using System.Reflection;
using log4net;

namespace MultiCommandConsole.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var assembly = typeof (Program).Assembly;
			log4net.Config.XmlConfigurator.ConfigureAndWatch(GetLogFileInfo(assembly));
			Config.CommandPromptText = "console";
			Config.ShowConsoleCommand = true;
			Config.ShowVierArgsCommand = true;
			Config.GetLoggerDelegate = type => new Log4NetLogger(LogManager.GetLogger(type));
			var engine = new Engine(new[] {assembly})
			             	{
			             		AppName="example_console",
								HistorySize = 5
			             	};
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
