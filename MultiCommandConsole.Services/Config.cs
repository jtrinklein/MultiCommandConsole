using System;
using System.ServiceProcess;
using Common.Logging;

namespace MultiCommandConsole.Services
{
    public static class Config
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Config));

        public static void EnableServiceMode()
        {
            if (!Environment.UserInteractive)
            {
                MultiCommandConsole.Config.GetRunnerDelegate =
                    repository => new ServiceCommandRunner(new CommandRunner(repository));

                Log.Info("Service mode enabled");
            }
            else
            {
                Log.Info("Service mode not enabled");
            }
        }

        public static class Defaults
        {
            /// <summary>The command line args used to run the command that will run as a service</summary>
            public static string CommandLine { get; set; }

            public static string ServiceNamePrefix { get; set; }
            public static ServiceAccount Account { get; set; }
            public static string Username { get; set; }
            public static string Password { get; set; }
            public static ServiceStartMode StartMode { get; set; }

            static Defaults()
            {
                Account = ServiceAccount.LocalSystem;
                StartMode = ServiceStartMode.Automatic;
            }
        }

    }
}