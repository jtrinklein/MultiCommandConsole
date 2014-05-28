using System.Reflection;

namespace MultiCommandConsole
{
    public class Arg
	{
		public PropertyInfo PropertyInfo { get; set; }
		public ArgAttribute ArgAttribute { get; set; }
		public ArgSetAttribute ArgSetAttribute { get; set; }

		public Arg(PropertyInfo propertyInfo, ArgAttribute arg, ArgSetAttribute argSet)
		{
			PropertyInfo = propertyInfo;
			ArgAttribute = arg;
			ArgSetAttribute = argSet;
		}
	}
}