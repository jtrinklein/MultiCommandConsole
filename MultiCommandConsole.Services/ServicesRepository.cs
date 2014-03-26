using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using Common.Logging;

namespace MultiCommandConsole.Services
{
    public class ServicesRepository : IServicesRepository
    {
        private static readonly ILog Log = LogManager.GetLogger<ServicesRepository>();

        private static bool Exists(string serviceName)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == serviceName);
        }

        public IEnumerable<ServiceController> List(string serviceName)
        {
            var regex = new Regex(serviceName, RegexOptions.IgnoreCase);
            return ServiceController.GetServices().Where(s => regex.IsMatch(s.ServiceName));
        }

        public void Save(Service options)
        {
            if (Exists(options.ServiceName))
            {
                Delete(options.ServiceName);
            }

            Add(options);
        }

        public void Add(Service options)
        {
            if (Exists(options.ServiceName))
            {
                throw new InvalidOperationException("service already exists with the name '" + options.ServiceName + "'");
            }

            CustomInstaller.Service = options;
            ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
            Log.InfoFormat("installed {0}", options.ServiceName);
        }

        public void Delete(string serviceName)
        {
            CustomInstaller.Service = new Service{ServiceName = serviceName};
            ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
            Log.InfoFormat("deleted {0}", serviceName);
        }
    }
}