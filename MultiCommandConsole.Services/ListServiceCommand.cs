﻿using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole.Services
{
    [ConsoleCommand("list-service", "lists the services matching the provided service name regex")]
    public class ListServiceCommand : IConsoleCommand
    {
        private readonly IServicesRepository _servicesRepository;
        internal IConsoleWriter Writer { get; set; }

        [Arg("pid", "process id of service to display")]
        public int ProcessId { get; set; }

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
            if (ProcessId > 0)
            {
                var service = _servicesRepository.GetByProcessId(ProcessId);
                Writer.WriteLine(service.Dump());
                return;
            }

            var services = _servicesRepository.All().ToList();
            foreach (var service in services.OrderBy(s => s.DisplayName))
            {
                Writer.WriteLine(service.DisplayName);
            }
            //TODO:
            // - validate their command line settings are still valid (add arg to let user decide if we validate or not)
        }
    }
}