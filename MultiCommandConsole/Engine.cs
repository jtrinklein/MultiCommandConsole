using System;
using System.Collections.Generic;
using System.Reflection;

namespace MultiCommandConsole
{
    public class Engine
    {
		private readonly ConsoleCommandRepository _commandRepository;

		public Engine(IEnumerable<Type> types)
		{
			_commandRepository = new ConsoleCommandRepository();
            _commandRepository.AddCommands(types);
		}

		public Engine(IEnumerable<Assembly> assemblies)
		{
			_commandRepository = new ConsoleCommandRepository();
			_commandRepository.AddCommands(assemblies);
		}

        public void Run(string[] args)
        {
            Config.GetRunnerDelegate(_commandRepository).Run(args);
        }
    }
}