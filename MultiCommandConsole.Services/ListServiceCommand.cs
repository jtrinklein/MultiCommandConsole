using System.Collections.Generic;
using System.Linq;

namespace MultiCommandConsole.Services
{
    [ConsoleCommand("install", "installs the current exe as a service targetting the specified command")]
    public class ListServiceCommand : IConsoleCommand
    {
        [Arg("servicename|sn", "service name")]
        public string ServiceName { get; set; }

        public ListServiceCommand()
        {
            ServiceName = Config.Defaults.ServiceNamePrefix;
        }

        public IEnumerable<string> GetArgValidationErrors()
        {
            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                return new[] { "servicename is required" };
            }
            return Enumerable.Empty<string>();
        }

        public string GetDetailedHelp()
        {
            return "";
        }

        public List<string> ExtraArgs { get; set; }

        public void Run()
        {
            var serviceControllers = new ServicesRepository().List(ServiceName);
            //TODO:
            // - list services alphabetically
            // - validate their command line settings are still valid (add arg to let user decide if we validate or not)
        }
    }
}