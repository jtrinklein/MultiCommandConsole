using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Services
{
    [ConsoleCommand("install-service", "installs the current exe as a service targetting the specified command")]
    public class InstallServiceCommand : IConsoleCommand
    {
        private readonly IServicesRepository _servicesRepository;
        private CommandRunData _commandRunData;
        private ICanRunAsService _serviceCommand;
        internal IConsoleWriter Writer { get; set; }

        public CommandsOptions CommandsOptions { get; set; }
        public AlterDataOptions AlterDataOptions { get; set; }

        [Arg("list", "list the commands that can be installed as services")]
        public bool List { get; set; }

        [Arg("servicename|sn", "overrides the service name")]
        public string ServiceName { get; set; }

        [Arg("displayname|dn", "overrides the display name")]
        public string DisplayName { get; set; }

        [Arg("description|d", "overrides the description for the service")]
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

        [Arg("command|c", "the command and arguments to run.  should be the last argument provided")]
        public string CommandLine { get; set; }

        public InstallServiceCommand() 
            : this(new ServicesRepository())
        {
        }

        public InstallServiceCommand(IServicesRepository servicesRepository)
        {
            _servicesRepository = servicesRepository;

            CommandLine = Config.Defaults.CommandLine;

            Account = Config.Defaults.Account;
            Username = Config.Defaults.Username;
            Password = Config.Defaults.Password;
            StartMode = Config.Defaults.StartMode;
        }

        public IEnumerable<string> GetArgValidationErrors()
        {
            var errors = new List<string>();
            if (List)
            {
                return errors;
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
            if (string.IsNullOrWhiteSpace(CommandLine))
            {
                errors.Add("command is required");
            }
            else
            {
                if (ExtraArgs.Count > 0)
                {
                    CommandLine = CommandLine.TrimEnd() + " " + string.Join(" ", ExtraArgs);
                    ExtraArgs.Clear();
                }

                _commandRunData = CommandsOptions.Load(CommandLine);

                if (_commandRunData.Errors.Count > 0)
                {
                    errors.Add("errors with arguments for /command");
                    errors.AddRange(_commandRunData.Errors);
                }
                else
                {
                    _serviceCommand = _commandRunData.Command as ICanRunAsService;
                    if (_serviceCommand == null)
                    {
                        errors.Add(_commandRunData.Command.GetType().Name + " does not implement ICanRunAsService");
                    }
                    else
                    {
                        ServiceName = _serviceCommand.ServiceName;
                        DisplayName = _serviceCommand.DisplayName;
                        Description = _serviceCommand.Description;
                    }
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
            if (List)
            {
                ListCommandsThatCanBeRunAsService();
                return;
            }
            
            var service = new Service
                {
                    ServiceName = ServiceName,
                    DisplayName = DisplayName,
                    Description = Description,
                    Account = Account,
                    Username = Username,
                    Password = Password,
                    StartMode = StartMode,
                    CommandLine = CommandLine
                };

            Writer.WriteLine("You are about to install this service:");
            Writer.WriteLine("  service name: " + service.ServiceName);
            Writer.WriteLine("  display name: " + service.DisplayName);
            Writer.WriteLine("  description : " + service.Description);
           
            if (!AlterDataOptions.Continue("Would you like to continue?"))
            {
                return;
            }

            if (ForceReinstall)
            {
                _servicesRepository.Save(service);
            }
            else
            {
                _servicesRepository.Add(service);
            }
        }

        private void ListCommandsThatCanBeRunAsService()
        {
            var commands = CommandsOptions.GetCommands()
                                          .Where(c => typeof (ICanRunAsService).IsAssignableFrom(c.CommandType))
                                          .ToList();

            if (commands.Count == 0)
            {
                Writer.WriteLine("no commands implement ICanRunAsService");
            }
            else
            {
                foreach (var command in commands)
                {
                    Writer.WriteLine(command.Attribute.FirstPrototype);
                }
            }
        }
    }
}
