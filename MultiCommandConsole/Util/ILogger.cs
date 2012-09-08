namespace MultiCommandConsole.Util
{
	public interface ILogger
	{
		void CurrentCommand(IConsoleCommand command);
		void Debug(string message);
		void DebugFormat(string format, params object[] args);
		void InfoFormat(string format, params object[] args);
		void Error(string message);
	}
}