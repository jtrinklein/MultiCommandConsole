using System;
using System.Collections.Generic;
using System.ServiceProcess;
using MultiCommandConsole.Commands;

namespace MultiCommandConsole.Services
{
    [ConsoleCommand("install", "installs the current exe as a service targetting the specified command")]
    public class InstallServiceCommand : IConsoleCommand
    {
        public CommandsOptions CommandsOptions { get; set; }

        [Arg("servicename|sn", "service name")]
        public string ServiceName { get; set; }

        [Arg("displayname|dn", "display name")]
        public string DisplayName { get; set; }

        [Arg("description|d", "description for the service")]
        public string Description { get; set; }

        [Arg("account|a", "the account the service will run as. options:LocalService,NetworkService,LocalSystem,User")]
        public ServiceAccount Account { get; set; }

        [Arg("user", "username used when account=User")]
        public string Username { get; set; }

        [Arg("pwd", "password used when account=User")]
        public string Password { get; set; }

        [Arg("startmode|sm", "options:Automatic,Manual,Disabled")]
        public ServiceStartMode StartMode { get; set; }

        [Arg("force|f", "uninstalls the service if it's already installed")]
        public bool ForceReinstall { get; set; }

        public InstallServiceCommand()
        {
            //TODO: load from CommandLine
            //ServiceName = Service.Default.ServiceName;
            //DisplayName = Service.Default.DisplayName;
            //Description = Service.Default.Description;

            Account = Config.Defaults.Account;
            Username = Config.Defaults.Username;
            Password = Config.Defaults.Password;
            StartMode = Config.Defaults.StartMode;
        }

        public IEnumerable<string> GetArgValidationErrors()
        {
            var errors = new List<string>();
            if (!Enum.IsDefined(typeof(ServiceAccount), Account))
            {
                errors.Add("invalid account");
            }
            if (!Enum.IsDefined(typeof(ServiceStartMode), StartMode))
            {
                errors.Add("invalid startmode");
            }
            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                errors.Add("servicename is required");
            }
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                errors.Add("displayname is required");
            }
            if (Account == ServiceAccount.User)
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    errors.Add("user is required when account=User");
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    errors.Add("pwd is required when account=User");
                }
            }
            return errors;
        }

        public string GetDetailedHelp()
        {
            //TODO: show Config.Defaults.CommandLine here
            //      explain how to just pass command line as last argument
            return "";
        }

        public List<string> ExtraArgs { get; set; }

        public void Run()
        {
            var repo = new ServicesRepository();
            var service = new Service
                {
                    ServiceName = ServiceName,
                    DisplayName = DisplayName,
                    Description = Description,
                    Account = Account,
                    Username = Username,
                    Password = Password,
                    StartMode = StartMode
                };

            //TODO:
            // - list commands available to install
            // - load command from args
            // - validate no errors
            // - add command line from extra args

            if (ForceReinstall)
            {
                repo.Save(service);
            }
            else
            {
                repo.Add(service);
            }
        }
    }
}
