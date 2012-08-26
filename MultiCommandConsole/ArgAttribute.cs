using System;

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
	}
}