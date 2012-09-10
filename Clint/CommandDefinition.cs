using System;
using System.Collections.Generic;

namespace Clint
{
	public interface ICommandDefinition : IDisplayHelp
	{
		IEnumerable<IOptionDefinition> OptionDefinitions { get; set; }
		Action Execute { get; set; }
	}

	public class CommandDefinition : ICommandDefinition, IDisplayHelpUsingDictionary
    {
		public IEnumerable<string> Aliases { get; set; }
		public IDictionary<string, object> Help { get; set; }
		public object GetHelp(string key)
		{
			return this.GetHelpFromDictionary(key);
		}

		public IEnumerable<IOptionDefinition> OptionDefinitions { get; set; }
		public Action Execute { get; set; }
    }
}
