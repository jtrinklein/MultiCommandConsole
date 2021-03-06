﻿using System.Collections.Generic;
using System.Linq;

namespace MultiCommandConsole.Services
{
    [ConsoleCommand("uninstall-service", "uninstalls the specified service")]
    public class UninstallServiceCommand : IConsoleCommand
    {
        [Arg("servicename|sn", "service name")]
        public string ServiceName { get; set; }

        public IEnumerable<string> GetArgValidationErrors()
        {
            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                return new[]{"servicename is required"};
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
            new ServicesRepository().Delete(ServiceName);
        }
    }
}