using System.Linq;
using Clint.Extensions;

namespace Clint
{
	public static class DisplayHelpExtensions
	{
		internal static object GetHelpFromDictionary(this IDisplayHelpUsingDictionary iDisplayHelp, string key)
		{
			var helByKey = iDisplayHelp.Help;
			if (helByKey.IsNullOrEmpty())
			{
				throw new InvalidConfigurationException(string.Format("Help was never specified for {1}: {0}", iDisplayHelp.GetName(), iDisplayHelp.GetType()));
			}
			object help;
			if (!helByKey.TryGetValue(key, out help))
			{
				throw new InvalidConfigurationException(string.Format("Help key '{0}' was never specified for {2}: {1}", key, iDisplayHelp.GetName(), iDisplayHelp.GetType()));
			}
			return help;
		}

		public static string GetName(this IDisplayHelp iDisplayHelp)
		{
			if(iDisplayHelp.Aliases.IsNullOrEmpty())
			{
				throw new InvalidConfigurationException(string.Format("Aliases have not been specified for: {0}", iDisplayHelp.GetType()));
			}
			return iDisplayHelp.Aliases.First();
		}
	}
}