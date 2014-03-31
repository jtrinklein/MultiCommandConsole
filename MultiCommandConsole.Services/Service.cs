using System;
using System.ServiceProcess;
using Common.Logging;

namespace MultiCommandConsole.Services
{
    public class Service
    {
        private static readonly ILog Log = LogManager.GetLogger<Service>();

        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ServiceAccount Account { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public string CommandLine { get; set; }
        public int ProcessId { get; set; }

        public string DescriptionWithCommandLine
        {
            get { return string.Format("{0}\n>>>{1}", Description, CommandLine); }
            set
            {
                var parts = value.Split(new[] {"\n>>>"}, StringSplitOptions.RemoveEmptyEntries);
                Log.DebugFormat("Description={0}", Description);
                Log.DebugFormat("CommandLine={0}", CommandLine);
                if (parts.Length > 0)
                {
                    Description = parts[0];
                }
                if (parts.Length > 1)
                {
                    CommandLine = parts[1];
                }
            }
        }

        public Service()
        {
            Account = Config.Defaults.Account;
            StartMode = Config.Defaults.StartMode;
        }
    }
}