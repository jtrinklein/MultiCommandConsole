using MultiCommandConsole.Util;
using ObjectPrinter;
using log4net;

namespace MultiCommandConsole.Example
{
	public class Log4NetLogger : ILogger
	{
		private readonly ILog _logger;

		public Log4NetLogger(ILog logger)
		{
			_logger = logger;
		}

		public void CurrentCommand(IConsoleCommand command)
		{
			_logger.DebugFormat("Running command {0}", command.DumpToLazyString());
		}

		public void Debug(string message)
		{
			_logger.Debug(message);
		}
		public void DebugFormat(string format, params object[] args)
		{
			_logger.DebugFormat(format, args);
		}

		public void InfoFormat(string format, params object[] args)
		{
			_logger.InfoFormat(format, args);
		}

		public void Error(string message)
		{
			_logger.Error(message);
		}
	}
}