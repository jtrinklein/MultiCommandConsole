using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MultiCommandConsole
{
    public class Engine
    {
		private readonly ConsoleCommandRepository _commandRepository;

		/// <summary>
		/// <para>
		/// A list of types the the Engine may have to resolve to run any of the commands 
		/// from the assemblies and types given in the ctor.
        /// </para>
		/// <para>
		/// Iterate through this list to wire up your IoC container.
        /// </para>
		/// </summary>
		public IEnumerable<Type> TypesToResolve
		{
			get
			{
				var commands = _commandRepository.Commands;
				foreach (var command in commands)
				{
					yield return command.CommandType;
					foreach (var arg in ArgsHelper.GetOptions(command.CommandType).Where(a => a.ArgSetAttribute != null))
					{
						yield return arg.PropertyInfo.PropertyType;
					}
				}
			}
		}

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

        public ICommandRunner GetRunner()
        {
            return new CommandRunner(_commandRepository);
        }
    }
}