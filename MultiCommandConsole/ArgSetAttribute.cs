using System;

namespace MultiCommandConsole
{
	public class ArgSetAttribute : Attribute
	{
		public ArgSetAttribute(string name, string descripion)
		{
			Name = name;
			Descripion = descripion;
		}

		public string Name { get; private set; }
		public string Descripion { get; private set; }
	}
}
