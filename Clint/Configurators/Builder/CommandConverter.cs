using System;
using System.Collections.Generic;
using System.Linq;

namespace Clint.Configurators.Builder
{
	public interface ICommandConverter
	{
		ICommandDefinition Convert(Command command);
	}

	public class CommandConverter : ICommandConverter
	{
		private readonly IOptionConverter _optionConverter;
		private char[] _aliasSeparator;
		private string _descriptionHelpKey;

		public CommandConverter(char[] aliasSeparator, string descriptionHelpKey, IOptionConverter optionConverter)
		{
			if (aliasSeparator == null) throw new ArgumentNullException("aliasSeparator");
			if (string.IsNullOrWhiteSpace(descriptionHelpKey)) throw new ArgumentNullException("descriptionHelpKey");
			if (optionConverter == null) throw new ArgumentNullException("optionConverter");
			_aliasSeparator = aliasSeparator;
			_descriptionHelpKey = descriptionHelpKey;
			_optionConverter = optionConverter;
		}

		public ICommandDefinition Convert(Command command)
		{
			var definition = new CommandDefinition
				{
					Aliases = command.Aliases.Split(_aliasSeparator, StringSplitOptions.RemoveEmptyEntries),
					Execute = command.Execute,
					Help = new Dictionary<string, object>(),
				};
			if (!string.IsNullOrWhiteSpace(command.Desc))
			{
				definition.Help[_descriptionHelpKey] = command.Desc;
			}
			int position = 1;
			definition.OptionDefinitions = command.Select(option =>
				{
					var o = _optionConverter.Convert(option);
					o.Position = position++;
					return o;
				});
			return definition;
		}
	}
}