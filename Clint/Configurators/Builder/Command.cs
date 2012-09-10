using System;
using System.Collections.Generic;

namespace Clint.Configurators.Builder
{
	public class Command : List<Option>
	{
		public string Aliases { get; private set; }
		public string Desc { get; set; }
		public Action Execute { get; set; }

		public Command()
		{
		}

		public Command(string aliases)
		{
			if (string.IsNullOrWhiteSpace(aliases)) throw new ArgumentNullException("aliases");
			Aliases = aliases;
		}
	}
}