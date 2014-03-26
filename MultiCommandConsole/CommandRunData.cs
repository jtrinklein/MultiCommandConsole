using System.Collections.Generic;

namespace MultiCommandConsole
{
	public class CommandRunData
	{
		public IConsoleCommand Command { get; set; }
		public List<ISetupAndCleanup> SetterUppers { get; set; }
        public List<string> Errors { get; set; } 

		public CommandRunData()
		{
			SetterUppers = new List<ISetupAndCleanup>();
            Errors = new List<string>();
		}
	}
}