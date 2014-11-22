using System;
using System.Collections.Generic;

namespace MultiCommandConsole
{
    public interface IConsoleCommandRepository
    {
        IEnumerable<ConsoleCommandInfo> Commands { get; }
        ConsoleCommandInfo GetByPrototype(string name);
        ConsoleCommandInfo GetByType(Type type);
        CommandRunData LoadCommand(string[] args);
        IDisposable HideCommandOfType<T>() where T : IConsoleCommand;
        CommandRunData LoadCommand<T>();
    }
}