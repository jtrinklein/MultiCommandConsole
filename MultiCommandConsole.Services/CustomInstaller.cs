using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace MultiCommandConsole.Services
{
    [RunInstaller(true)]
    public class CustomInstaller : Installer
    {
        private readonly Service _service;

        public CustomInstaller(Service service)
        {
            _service = service;
            Installers.Add(GetServiceInstaller());
            Installers.Add(GetServiceProcessInstaller());
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Context = new InstallContext(
                null, 
                new[]
                    {
                        "/LogToConsole=true", 
                        "/InstallStateDir=" + Path.Combine(baseDir, "service-install.state"),
                        "/LogFile=" + Path.Combine(baseDir, "service-install.log")
                    });

            foreach (string key in Context.Parameters.Keys)
            {
                Console.Out.WriteLine("{0}={1}", key, Context.Parameters[key]);
            }
        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
            Context.Parameters["assemblypath"] = "\"" + Assembly.GetEntryAssembly().Location + "\"";
        }

        private ServiceInstaller GetServiceInstaller()
        {
            return new ServiceInstaller
                {
                    ServiceName = _service.ServiceName,
                    DisplayName = _service.DisplayName,
                    Description = _service.Description,
                    StartType = _service.StartMode,
                };
        }

        private ServiceProcessInstaller GetServiceProcessInstaller()
        {
            return _service.Account == ServiceAccount.User
                       ? new ServiceProcessInstaller
                           {
                               Account = _service.Account,
                               Username = _service.Username,
                               Password = _service.Password
                           }
                       : new ServiceProcessInstaller
                           {
                               Account = _service.Account
                           };
        }
    }
}