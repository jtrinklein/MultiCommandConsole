using System;
using System.Collections.Generic;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;

namespace MultiCommandConsole
{
    /// <summary>
    /// The entry point for MultiCommandConsole configuration
    /// </summary>
	public static class Config
    {
        /// <summary>
        /// Configs to support ConsoleMode
        /// </summary>
        public static class ConsoleMode
        {
            /// <summary>
            /// If true, an additional command will be displayed to allow the user to enter interactive console mode
            /// </summary>
            public static bool Enabled { get; set; }

            /// <summary>
            /// The prompt text displayed before the prompt
            /// </summary>
            public static string CommandPromptText { get; set; }

            /// <summary>The name of the application.  Will be used to store console history.</summary>
            public static string AppName { get; set; }

            /// <summary>
            /// The number of entries to keep in history
            /// </summary>
            public static int? HistorySize { get; set; }

            /// <summary>
            /// Hook to allow modifying arguments before they're processed
            /// </summary>
            public static Func<string[], string[]> ArgumentsInterceptor { get; set; }

            /// <summary>
            /// Provides a way to setup an environment before a command is run and cleanup after.
            /// <hint>This is where you should setup any DI container lifecycles</hint>.
            /// </summary>
            public static Func<IDisposable> OnWrapRunCommand { get; set; }

            /// <summary>
            /// Indicates a command is about to be run.
            /// </summary>
            public static Action<IConsoleCommand> OnBeginRunCommand { get; set; }

            /// <summary>
            /// Indicates a command has finished running.  
            /// <hint>This is where you should cleanup any DI container lifecycles</hint>.
            /// </summary>
            public static Action<RunTime> OnEndRunCommand { get; set; }
        }

        /// <summary>
        /// Configs for augmenting help
        /// </summary>
        public static class Help
        {
            /// <summary>
            /// Allows specifying a category for a command.  
            /// The first prototype argument and the type will be returned.
            /// </summary>
            public static Func<string, Type, string> GetCategoryDelegate { get; set; }

            /// <summary>
            /// Allows specifying additional help lines for a command
            /// </summary>
            public static Func<string, Type, IEnumerable<string>> GetAddlHelpDelegate { get; set; }
        }

	    private static Type _defaultCommand = typeof(HelpCommand);
	    private static Func<DateTime> _nowDelegate = () => DateTime.Now;

	    /// <summary>
        /// When not specified or set to null, the help command becomes the default command.
        /// </summary>
        public static Type DefaultCommand
	    {
	        get { return _defaultCommand; }
	        set { _defaultCommand = value ?? typeof(HelpCommand); }
	    }

        /// <summary>
        /// When true, the start and stop times as well as the run time will be output to console
        /// </summary>
        public static bool WriteRunTimeToConsole { get; set; }

        /// <summary>
        /// The console writer used to write messages for the console window 
        /// (or to the logger when in running as a service)
        /// </summary>
        [Obsolete("use GetConsoleWriterByTypeDelegate")] //2014-10-06  v.2.0.34
        public static Func<Type, IConsoleWriter> GetConsoleWriterDelegate
        {
            get { return GetConsoleWriterByTypeDelegate; }
            set { GetConsoleWriterByTypeDelegate = value; }
        }

        /// <summary>
        /// The console writer used to write messages for the console window 
        /// (or to the logger when running as a service)
        /// </summary>
        public static Func<Type, IConsoleWriter> GetConsoleWriterByTypeDelegate { get; set; }

        /// <summary>
        /// The console writer used to write messages for the console window 
        /// (or to the logger when running as a service)
        /// </summary>
        public static Func<string, IConsoleWriter> GetConsoleWriterByNameDelegate { get; set; }

	    /// <summary>
		/// By default, commands and arg sets are created by Activator.  
		/// Use this delegate to override the default behavior. 
		/// </summary>
		public static Func<Type, object> ResolveTypeDelegate { get; set; }

		/// <summary>
		/// If true, an additional command will be displayed to allow the user to see how the text they 
		/// enter is converted into arguments.  Useful when arguments need to escape quotes.
		/// </summary>
		public static bool ShowViewArgsCommand { get; set; }

		/// <summary>
		/// In a standard dos console, Escape clears the current line. 
		/// Setting EscapeIsAltKey to true will cause Escape to trigger the Alt modifier 
		/// instead of clearing the current line.  This is similar to some *nix consoles.
		/// </summary>
		public static bool EscapeIsAltKey { get; set; }

        /// <summary>
        /// Delegate used to determine the current datetime. Defaults to DateTime.Now and setting to null resets to DateTime.Now
        /// </summary>
        public static Func<DateTime> NowDelegate
        {
            get { return _nowDelegate; }
            set { _nowDelegate = value ?? (() => DateTime.Now); }
        }

        /// <summary>
        /// Delegate to return a CommandRunner.  
        /// Defaults to using CommandRunner.  
        /// When ServiceMode is enabled, uses a ServiceBase runner.
        /// </summary>
        public static Func<IConsoleCommandRepository, ICommandRunner> GetRunnerDelegate { get; set; }

	    static Config()
		{
			ShowViewArgsCommand = false;
		    ResolveTypeDelegate = Activator.CreateInstance;

            ConsoleMode.CommandPromptText = "$";
		    ConsoleMode.HistorySize = 50;

            GetConsoleWriterByTypeDelegate = type => new ConsoleWriter();
            GetConsoleWriterByNameDelegate = name => new ConsoleWriter();

            GetRunnerDelegate = repository => new CommandRunner(repository);
		}
	}
}