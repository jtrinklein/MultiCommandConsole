using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace MultiCommandConsole.Example
{
	[ArgSet("logger", "manages verbosity level so commands don't have to")]
	public class LogOptions : IValidatable, ISetupAndCleanup
	{
		private static readonly Dictionary<string, Level> Levels =
			new Dictionary<string, Level>(StringComparer.OrdinalIgnoreCase)
				{
					{Level.Fatal.Name, Level.Fatal},
					{Level.Error.Name, Level.Error},
					{Level.Warn.Name, Level.Warn},
					{"Output", Level.Notice}, //this is the level the UI should output standard results to
					{Level.Info.Name, Level.Info},
					{Level.Debug.Name, Level.Debug},
				};
		private static Hierarchy _hierarchy = (Hierarchy)LogManager.GetRepository();
		private static Logger _rootLogger = _hierarchy.Root;
		private static Level _origRootLogLevel = _rootLogger.Level;
		private Level _verbosity = Level.Info;
		private AppenderSkeleton _consoleAppender;

		[Arg("quiet", "when specified, only errors will be logged")]
		public bool Quiet { get; set; }
		[Arg("verbose", "when specified, everything is logged.")]
		public bool Verbose { get; set; }
		[Arg("loglevel", "allows specifying the level of verbosity to log to the client.  Valid values in order: Fatal,Error,Warn,Output,Info,Debug")]
		public string Verbosity { get; set; }


		public IEnumerable<string> GetArgValidationErrors()
		{
			if (Quiet && Verbose)
			{
				return new[] { "quiet and verbose were both specified. Pick one or the other." };
			}
			if (!string.IsNullOrEmpty(Verbosity))
			{
				if (Quiet)
				{
					return new[] { "quiet was specified while also specifying loglevel. Pick one or the other." };
				}
				if (Verbose)
				{
					return new[] { "verbose was specified while also specifying loglevel. Pick one or the other." };
				}
				if (!Levels.TryGetValue(Verbosity, out _verbosity))
				{
					return new[] { "Invalid loglevel: " + Verbosity };
				}
			}
			return Enumerable.Empty<string>();
		}

		public void Setup()
		{
			EnsureConsoleAppender();

			if (Quiet)
			{
				_verbosity = Level.Notice;
			}
			else if (Verbose)
			{
				_verbosity = Level.All;
			}
			_consoleAppender.Threshold = _verbosity;
			if (_rootLogger.Level == null || _verbosity < _rootLogger.Level)
			{
				_rootLogger.Level = _verbosity;
			}

			_hierarchy.Configured = true;
		}

		public void Cleanup()
		{
			_rootLogger.Level = _origRootLogLevel;
			_hierarchy.Configured = true;
		}

		private void EnsureConsoleAppender()
		{
			if (_consoleAppender != null)
			{
				return;
			}

			var consoleAppenderName = "ConsoleAppender";
			_consoleAppender = (AppenderSkeleton)_rootLogger.GetAppender(consoleAppenderName);
			if (_consoleAppender == null)
			{
				_consoleAppender = GetConfiguredAppender(consoleAppenderName) ?? CreateAppender(consoleAppenderName);
				_rootLogger.AddAppender(_consoleAppender);
			}
		}

		private static AppenderSkeleton GetConfiguredAppender(string consoleAppenderName)
		{
			var appenders = _hierarchy.GetAppenders();
			return (AppenderSkeleton)appenders.FirstOrDefault(
				a => consoleAppenderName.Equals(a.Name, StringComparison.OrdinalIgnoreCase));
		}

		private static ColoredConsoleAppender CreateAppender(string consoleAppenderName)
		{
			var newAppender = new ColoredConsoleAppender
			{
				Name = consoleAppenderName,
				Layout = new PatternLayout("%message%newline")
			};
			newAppender.AddMapping(new ColoredConsoleAppender.LevelColors
			{
				Level = Level.Error,
				ForeColor = ColoredConsoleAppender.Colors.Red
			});
			return newAppender;
		}
	}
}