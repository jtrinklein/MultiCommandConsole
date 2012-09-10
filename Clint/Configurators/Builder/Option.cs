using System;

namespace Clint.Configurators.Builder
{
	public class Option
	{
		public string Aliases { get; private set; }
		public string Desc { get; set; }
		public Action<string> Action { get; private set; }

		public Option(string aliases, Action<string> action)
		{
			if (string.IsNullOrWhiteSpace(aliases)) throw new ArgumentNullException("aliases");
			if (action == null) throw new ArgumentNullException("action");

			Aliases = aliases;
			Action = action;
		}
	}
}