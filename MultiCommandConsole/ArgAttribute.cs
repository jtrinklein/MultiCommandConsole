using System;
using System.Linq;
using MultiCommandConsole.Util;

namespace MultiCommandConsole
{
	public class ArgAttribute : Attribute
	{
		public ArgAttribute(string prototype, string description)
		{
			Prototype = prototype;
			Description = description;
		}

		public string Prototype { get; private set; }
		public string Description { get; private set; }
		public bool Required { get; set; }
		public string AppSettingsKey { get; set; }
		public string ConnectionStringKey { get; set; }

        public string[] PrototypeArray { get { return Prototype.GetPrototypeArray(); } }
        public string FirstPrototype { get { return PrototypeArray.First(); } }
	}
}