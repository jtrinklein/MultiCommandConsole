namespace MultiCommandConsole.Util
{
	public class NullLogger : ILogger
	{
		public void CurrentCommand(IConsoleCommand command)
		{
			/*no op*/
		}

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