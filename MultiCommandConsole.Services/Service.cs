using System.ServiceProcess;

namespace MultiCommandConsole.Services
{
    public class Service
    {
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ServiceAccount Account { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public string CommandLine { get; set; }

        public Service()
        {
            Account = Config.Defaults.Account;
            StartMode = Config.Defaults.StartMode;
        }
    }
}