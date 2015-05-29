using System;
using System.Diagnostics;
using System.Threading;
using Common.Logging;
using MultiCommandConsole.Services;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole.Example
{
	class Program
    {
        private static IConsoleWriter _writer;
	    private static ILog _logger;

	    static void Main(string[] args)
		{
            if (Array.Exists(args, s => s == "/debug") && MCCEnvironment.UserInteractive)
            {
                Debugger.Break();
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            LogManager.Adapter = Log4NetFactoryAdapter.Load();
		    _logger = LogManager.GetLogger(typeof (Program));
            _logger.InfoFormat("Logging configured");

            Services.Config.EnableServiceMode();
		    _writer = ConsoleWriter.Get<Program>();

            Config.ConsoleMode.Enabled = true;
			Config.ConsoleMode.CommandPromptText = "console";
		    Config.ConsoleMode.HistorySize = 5;
		    Config.ConsoleMode.AppName = "example_console";
		    Config.ConsoleMode.OnBeginRunCommand += cmd => _writer.WriteLine(cmd.Dump());

			Config.ShowViewArgsCommand = true;
		    Config.WriteRunTimeToConsole = true;
		    //Config.DefaultCommand = typeof (RepeatConsoleCommand);
		    Config.Help.GetCategoryDelegate = (name, type) =>
		        {
		            if (name.EndsWith("-service"))
		            {
		                return " - service -";
		            }
                    if (type.Assembly == typeof (Program).Assembly)
                    {
                        return " - example -";
                    }
		            return "";
                };
            Config.Help.GetAddlHelpDelegate = (s, type) => new[] { type.FullName };

            Services.Config.Defaults.CommandLine = "ping /s=google.com /i=10";

		    try
		    {
		        new Engine(new[]
		            {
		                typeof(Program).Assembly,
		                typeof(InstallServiceCommand).Assembly
		            }).Run(args);
		    }
		    catch (Exception e)
		    {
		        _writer.WriteLine(e.Dump());
		    }
		}

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            try
            {
                _writer.WriteErrorLine(unhandledExceptionEventArgs.ExceptionObject.Dump());
            }
            catch (Exception e)
            {
                _logger.Fatal(unhandledExceptionEventArgs.ExceptionObject.Dump());
            }
            ((Stoplight)Config.ResolveTypeDelegate(typeof(Stoplight))).Stop();
        }
	}
}
