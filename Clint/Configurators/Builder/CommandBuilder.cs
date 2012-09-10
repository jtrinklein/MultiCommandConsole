using System;
using System.Collections.Generic;
using System.Linq;

namespace Clint.Configurators.Builder
{
	public class CommandBuilder
	{
		private readonly ICommandConverter _commandConverter;

		public CommandBuilder(ICommandConverter commandConverter)
		{
			if (commandConverter == null) throw new ArgumentNullException("commandConverter");
			_commandConverter = commandConverter;
		}
		
		public IEnumerable<ICommandDefinition> GetCommands(IEnumerable<Command> commands)
		{
			if (commands == null) throw new ArgumentNullException("commands");

			return commands.Select(_commandConverter.Convert);
		}
	}
}