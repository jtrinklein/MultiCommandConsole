using System;
using System.Collections.Generic;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Commands
{
    [ArgSet("commands", "provides all commands")]
    public class CommandsOptions
    {
        internal IConsoleCommandRepository ConsoleCommandRepository { private get; set; }

        public IEnumerable<ConsoleCommandInfo> GetCommands()
        {
            return ConsoleCommandRepository.Commands;
        }

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

        public CommandRunData Load(string commandLine)
        {
            return ConsoleCommandRepository.LoadCommand(commandLine.SplitCmdLineArgs());
        }
    }
}