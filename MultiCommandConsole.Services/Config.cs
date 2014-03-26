using System.ServiceProcess;

namespace MultiCommandConsole.Services
{
    public static class Config
    {
        public static class Defaults
        {
            /// <summary>The command line args used to run the command that will run as a service</summary>
            public static string CommandLine { get; set; }

            public static string ServiceNamePrefix { get; set; }
            public static ServiceAccount Account { get; set; }
            public static string Username { get; set; }
            public static string Password { get; set; }
            public static ServiceStartMode StartMode { get; set; }
        }

    }
}