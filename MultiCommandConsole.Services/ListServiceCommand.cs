using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Services
{
    [ConsoleCommand("list-service", "lists the services matching the provided service name regex")]
    public class ListServiceCommand : IConsoleCommand
    {
        private readonly IServicesRepository _servicesRepository;
        private IConsoleWriter _writer;

        public ListServiceCommand() 
            : this(new ServicesRepository())
        {
        }
        public ListServiceCommand(IServicesRepository servicesRepository)
        {
            _servicesRepository = servicesRepository;
        }

        public IEnumerable<string> GetArgValidationErrors()
        {
            return Enumerable.Empty<string>();
        }

        public string GetDetailedHelp()
        {
            return "";
        }

        public List<string> ExtraArgs { get; set; }

        public void Run()
        {
            var services = _servicesRepository.All().ToList();
            foreach (var service in services.OrderBy(s => s.DisplayName))
            {
                _writer.WriteLine(service.DisplayName);
            }
            //TODO:
            // - validate their command line settings are still valid (add arg to let user decide if we validate or not)
        }
    }
}