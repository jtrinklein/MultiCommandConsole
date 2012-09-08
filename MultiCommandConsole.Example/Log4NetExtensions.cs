using System;
using System.Globalization;
using log4net;
using log4net.Core;
using log4net.Util;

namespace MultiCommandConsole.Example
{
	public static class Log4NetExtensions
	{
		private readonly static Type ThisDeclaringType = typeof(Log4NetExtensions);
		public static void ConsoleOut(this ILog log, object message, Exception ex = null)
		{
			log.Logger.Log(ThisDeclaringType, Level.Notice, message, ex);
		}
		public static void ConsoleOutFormat(this ILog log, string format, params object[] args)
		{
			ConsoleOutFormat(log, format, null, null, args);
		}
		public static void ConsoleOutFormat(this ILog log, string format, Exception ex = null, IFormatProvider provider = null, params object[] args)
		{
			log.Logger.Log(ThisDeclaringType, Level.Notice, new SystemStringFormat(provider ?? CultureInfo.InvariantCulture, format, args), ex);
		}
	}
}