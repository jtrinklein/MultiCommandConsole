using System;
using System.ServiceProcess;
using Common.Logging;
using MultiCommandConsole.Util;
using MccConfig = MultiCommandConsole.Config;

namespace MultiCommandConsole.Services
{
    /// <summary>
    /// The entry point for MultiCommandConsole.Services configuration
    /// </summary>
    public static class Config
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Config));

        /// <summary>
        /// When called, overrides configs to support running as a service
        /// </summary>
        public static void EnableServiceMode()
        {
            if (Environment.UserInteractive)
            {
                Log.Info("Service mode not enabled. Environment.UserInteractive=false");
            }
            else
            {
                MccConfig.GetConsoleWriterByTypeDelegate = type => new LoggingConsoleWriter(type);
                MccConfig.GetConsoleWriterByNameDelegate = name => new LoggingConsoleWriter(name);
                MccConfig.GetRunnerDelegate =
                    repository => new ServiceCommandRunner(new CommandRunner(repository));

                Log.Info("Service mode enabled");
            }
        }

        /// <summary>
        /// Defaults
        /// </summary>
        public static class Defaults
        {
            /// <summary>The command line args used to run the command that will run as a service</summary>
            public static string CommandLine { get; set; }
            /// <summary>prefix to be added to all service names.  useful application prefix</summary>
            public static string ServiceNamePrefix { get; set; }
            /// <summary>the default account type (LocalSystem)</summary>
            public static ServiceAccount Account { get; set; }
            /// <summary>the default username, when Account is User</summary>
            public static string Username { get; set; }
            /// <summary>the default password, when Account is User</summary>
            public static string Password { get; set; }
            /// <summary>The default start mode (Automatic)</summary>
            public static ServiceStartMode StartMode { get; set; }

            static Defaults()
            {
                Account = ServiceAccount.LocalSystem;
                StartMode = ServiceStartMode.Automatic;
            }
        }

    }
}