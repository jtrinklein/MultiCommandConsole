using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace MultiCommandConsole.Services
{
    [RunInstaller(true)]
    public class CustomInstaller : Installer
    {
        public static Service Service { get; set; }

        public CustomInstaller()
        {
            Installers.Add(GetServiceProcessInstaller());
            Installers.Add(
                new ServiceInstaller
                    {
                        ServiceName = Service.ServiceName, //must be the same as what was set in Program's constructor
                        DisplayName = Service.DisplayName,
                        Description = Service.Description,
                        StartType = Service.StartMode,
                    });
        }

        private static ServiceProcessInstaller GetServiceProcessInstaller()
        {
            return Service.Account == ServiceAccount.User
                       ? new ServiceProcessInstaller
                           {
                               Account = Service.Account,
                               Username = Service.Username,
                               Password = Service.Password
                           }
                       : new ServiceProcessInstaller
                           {
                               Account = Service.Account
                           };
        }
    }
}