using System;
using System.Collections.Generic;

namespace MultiCommandConsole.Commands
{
    [ArgSet("commands", "provides all commands")]
    public class CommandsOptions
    {
        internal ConsoleCommandRepository ConsoleCommandRepository { private get; set; }

        public IEnumerable<ConsoleCommandInfo> Commands { get { return ConsoleCommandRepository.Commands; } }

        public ICommandRunner GetRunner()
        {
            return new CommandRunner(ConsoleCommandRepository);
        }

        public IDisposable HideCommandOfType<T>() where T: IConsoleCommand
        {
            return ConsoleCommandRepository.HideCommandOfType<T>();
        }

        public ConsoleCommandInfo GetByPrototype(string name)
        {
            return ConsoleCommandRepository.GetByPrototype(name);
        }

        public ConsoleCommandInfo GetByType(Type type)
        {
            return ConsoleCommandRepository.GetByType(type);
        }
    }
}