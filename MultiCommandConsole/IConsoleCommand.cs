using System.Collections.Generic;

namespace MultiCommandConsole
{
	public interface IConsoleCommand : IValidatable
	{
		string GetDetailedHelp();
		List<string> ExtraArgs { get; set; }
		void Run();
	}
}