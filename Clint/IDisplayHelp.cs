using System.Collections.Generic;

namespace Clint
{
	public interface IDisplayHelp
	{
		IEnumerable<string> Aliases { get; set; }

		/// <summary>Help to be displayed to users, keyed by level.  Examples could be "basic","detailed","examples","web"...  Null or Empty string should return default help</summary>
		object GetHelp(string key);
	}
}