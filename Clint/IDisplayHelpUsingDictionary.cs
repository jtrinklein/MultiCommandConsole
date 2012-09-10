using System.Collections.Generic;

namespace Clint
{
	public interface IDisplayHelpUsingDictionary : IDisplayHelp
	{
		IDictionary<string, object> Help { get; set; }
	}
}