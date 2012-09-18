using System;
using MultiCommandConsole.Util;

namespace MultiCommandConsole
{
	public static class Config
	{
		private static NullLogger _default;

		/// <summary>Use this to inject your own logging framework.</summary>
		public static Func<Type, ILogger> GetLoggerDelegate { get; set; }

		/// <summary>
		/// By default, commands and arg sets are created by Activator.  
		/// Use this delegate to override the default behavior. 
		/// </summary>
		public static Func<Type, object> ResolveTypeDelegate { get; set; }

		/// <summary>
		/// Indicates a new command is running.  <hint>This is where you should refresh any DI Container lifecycles</hint>.
		/// </summary>
		public static Action RunningNewCommand { get; set; }

		/// <summary>The console formatter used to format messages for the console window.</summary>
		public static ConsoleFormatter ConsoleFormatter { get; set; }

		/// <summary>The prompt text displayed before the prompt while in console mode.</summary>
		public static string CommandPromptText { get; set; }

		/// <summary>
		/// If true, an additional command will be displayed to allow the user to enter interactive console mode
		/// </summary>
		public static bool ShowConsoleCommand { get; set; }

		/// <summary>
		/// If true, an additional command will be displayed to allow the user to see how the text they 
		/// enter is converted into arguments.  Useful when arguments need to escape quotes.
		/// </summary>
		public static bool ShowVierArgsCommand { get; set; }

		/// <summary>
		/// In a standard dos console, Escape clears the current line. 
		/// Setting EscapeIsAltKey to true will cause Escape to trigger the Alt modifier 
		/// instead of clearing the current line.  This is similar to some *nix consoles.
		/// </summary>
		public static bool EscapeIsAltKey { get; set; }

		static Config()
		{
			GetLoggerDelegate = delegate { return _default ?? (_default = new NullLogger()); };
			ConsoleFormatter = new ConsoleFormatter();
			CommandPromptText = "$";
			ShowVierArgsCommand = false;
		}
	}
}