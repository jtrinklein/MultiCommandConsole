using System;

namespace MultiCommandConsole
{
	public class ConsoleCommandAttribute : Attribute
	{
		public ConsoleCommandAttribute(string prototype, string descripion)
		{
			Prototype = prototype;
			Descripion = descripion;
		}

		public string Prototype { get; private set; }
		public string Descripion { get; private set; }
	}
}