using System;
using System.Collections.Generic;
using System.Reflection;

namespace MultiCommandConsole
{
    /// <summary>
    /// Entry point to run commands
    /// </summary>
    public class Engine
    {
		private readonly ConsoleCommandRepository _commandRepository;

		/// <summary>
		/// ctors an instance with the provided command types
		/// </summary>
		public Engine(IEnumerable<Type> types)
		{
			_commandRepository = new ConsoleCommandRepository();
            _commandRepository.AddCommands(types);
		}

        /// <summary>
        /// ctors an instance with the provided commands from the provided assemblies
        /// </summary>
		public Engine(IEnumerable<Assembly> assemblies)
		{
			_commandRepository = new ConsoleCommandRepository();
			_commandRepository.AddCommands(assemblies);
		}

        /// <summary>
        /// runs the command for the given args
        /// </summary>
        public void Run(string[] args)
        {
            Config.GetRunnerDelegate(_commandRepository).Run(args);
        }
    }
}