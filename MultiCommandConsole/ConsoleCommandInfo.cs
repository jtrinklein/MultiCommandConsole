using System;

namespace MultiCommandConsole
{
	public class ConsoleCommandInfo
	{
		public ConsoleCommandAttribute Attribute { get; set; }
		public Type CommandType { get; set; }
		public IConsoleCommand Instance { get; set; }
	}
}