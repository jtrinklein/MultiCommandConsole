namespace MultiCommandConsole.Util
{
	internal static class Logging
	{
		public static ILogger GetLogger<T>()
		{
			return Config.GetLoggerDelegate(typeof(T));
		}
	}
}