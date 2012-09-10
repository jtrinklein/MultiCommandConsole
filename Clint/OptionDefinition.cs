using System;
using System.Collections.Generic;

namespace Clint
{
	public interface IOptionDefinition : IDisplayHelp
	{
		int Position { get; set; }
		object Default { get; set; }
		Action<string> OnParse { get; set; }
	}

	public class OptionDefinition : IOptionDefinition, IDisplayHelpUsingDictionary
	{
		public IEnumerable<string> Aliases { get; set; }
		public IDictionary<string, object> Help { get; set; }
		public object GetHelp(string key)
		{
			return this.GetHelpFromDictionary(key);
		}

		public int Position { get; set; }
		public object Default { get; set; }
		public Action<string> OnParse { get; set; }
	}
}