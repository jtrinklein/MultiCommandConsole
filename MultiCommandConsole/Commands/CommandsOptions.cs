using System;
using System.Collections.Generic;

namespace MultiCommandConsole.Commands
{
    [ArgSet("commands", "provides all commands")]
    public class CommandsOptions
    {
        internal Dictionary<string, ConsoleCommandInfo> CommandsByPrototype { private get; set; }
        internal Dictionary<Type, ConsoleCommandInfo> CommandsByType { private get; set; }

        public IEnumerable<ConsoleCommandInfo> Commands { get; set; }

        public ConsoleCommandInfo GetByPrototype(string name)
        {
            ConsoleCommandInfo info;
            return CommandsByPrototype.TryGetValue(name, out info) ? info : null;
        }

        public ConsoleCommandInfo GetByType(Type type)
        {
            ConsoleCommandInfo info;
            return CommandsByType.TryGetValue(type, out info) ? info : null;
        }
    }
}