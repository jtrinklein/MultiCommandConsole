namespace MultiCommandConsole.Util
{
	internal static class Logging
	{
		public static ILogger GetLogger<T>()
		{
			return Config.GetLoggerDelegate(typeof(T));
		}
	}

	public interface ILogger
	{
		void Debug(string message);
		void DebugFormat(string format, params object[] args);
		void InfoFormat(string format, params object[] args);
		void Error(string message);
	}

	public class NullLogger : ILogger
	{

		public void Debug(string message)
		{
			/*no op*/
		}
		public void DebugFormat(string format, params object[] args)
		{
			/*no op*/
		}

		public void InfoFormat(string format, params object[] args)
		{
			/*no op*/
		}

		public void Error(string message)
		{
			/*no op*/
		}
	}
}