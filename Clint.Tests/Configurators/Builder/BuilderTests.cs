using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clint.Configurators.Builder;
using Clint.Extensions;
using NUnit.Framework;

namespace Clint.Tests.Configurators.Builder
{
	[TestFixture]
	public class BuilderTests
    {
		/* TESTS:
		 *
		 * default command (no command specified - for single command cli apps)
		 * - no command aliases are required
		 * 
		 * several named commands
		 * - aliases are required for every command
		 * - duplicate aliases are not allowed
		 * 
		 * configure a command with
		 * - no args
		 * - 1 arg
		 * - several args
		 * - positional args
		 * - 
		 * 
		 */

		private static char[] aliasSeparator = new[] { '|' };
		private static string descriptionHelpKey = "descriptionHelpKey";

		CommandBuilder standardBuilder = new CommandBuilder(new CommandConverter(aliasSeparator, descriptionHelpKey, new OptionConverter(aliasSeparator, descriptionHelpKey)));

		private IEnumerable<ICommandDefinition> Build(params Command[] commands)
		{
			return Build(standardBuilder, commands);
		}
		private IEnumerable<ICommandDefinition> Build(CommandBuilder builder, params Command[] commands)
		{
			return builder.GetCommands(commands);
		}

		[Test]
		public void Should_be_able_to_create_single_command_with_no_aliases_specified()
		{
			bool showHelp = false;
			var commands = Build(
				new Command
					{
						new Option("help|?", s => showHelp = true) {Desc = "show helps"}
					}
				);
		}
    }

}
