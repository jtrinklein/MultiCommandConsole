using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Management;
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
            return ServiceController.GetServices().Any(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Service> All()
        {
            //using ServiceController only returns Name & DisplayName
            var mc = new ManagementClass("Win32_Service");
            return mc.GetInstances()
                     .Cast<ManagementObject>()
                     .Where(IsThisExecutable)
                     .Select(mo => new Service
                         {
                             ServiceName = mo.GetPropertyValue("Name").ToString(),
                             DisplayName = mo.GetPropertyValue("DisplayName").ToString(),
                             Description = mo.GetPropertyValue("Description").ToString(),
                         });
        }

        private static bool IsThisExecutable(ManagementObject mo)
        {
            var path = mo.GetPropertyValue("PathName").ToString();
            return path.Contains(Assembly.GetEntryAssembly().Location);
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

            new CustomInstaller(options).Install(new Hashtable());
            
            //ManagedInstallerClass.InstallHelper(new[] { Assembly.GetEntryAssembly().Location });
            Log.InfoFormat("installed {0}", options.ServiceName);
        }

        public void Delete(string serviceName)
        {
            new CustomInstaller(new Service{ServiceName = serviceName}).Uninstall(null);
            //ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
            Log.InfoFormat("deleted {0}", serviceName);
        }
    }
}