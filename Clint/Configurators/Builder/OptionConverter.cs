using System;
using System.Collections.Generic;

namespace Clint.Configurators.Builder
{
	public interface IOptionConverter
	{
		IOptionDefinition Convert(Option option);
	}

	public class OptionConverter : IOptionConverter
	{
		private readonly char[] _aliasSeparator;
		private readonly string _descriptionHelpKey;

		public OptionConverter(char[] aliasSeparator, string descriptionHelpKey)
		{
			if (aliasSeparator == null) throw new ArgumentNullException("aliasSeparator");
			if (string.IsNullOrWhiteSpace(descriptionHelpKey)) throw new ArgumentNullException("descriptionHelpKey");
			_aliasSeparator = aliasSeparator;
			_descriptionHelpKey = descriptionHelpKey;
		}

		public IOptionDefinition Convert(Option option)
		{
			var definition = new OptionDefinition
				{
					Aliases = option.Aliases.Split(_aliasSeparator, StringSplitOptions.RemoveEmptyEntries),
					OnParse = option.Action,
					Help = new Dictionary<string, object>(),
				};
			if (!string.IsNullOrWhiteSpace(option.Desc))
			{
				definition.Help[_descriptionHelpKey] = option.Desc;
			}
			return definition;
		}
	}
}