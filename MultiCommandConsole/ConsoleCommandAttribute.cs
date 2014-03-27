using System;
using System.Linq;
using MultiCommandConsole.Util;

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

        public string[] PrototypeArray { get { return Prototype.GetPrototypeArray(); } }
        public string FirstPrototype { get { return PrototypeArray.First(); } }
	}
}