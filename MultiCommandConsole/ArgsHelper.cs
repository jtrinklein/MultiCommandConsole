using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MultiCommandConsole
{
	public static class ArgsHelper
	{
		private static Dictionary<Type, IEnumerable<Arg>> _optionsCache = new Dictionary<Type, IEnumerable<Arg>>();

		public static IEnumerable<Arg> GetOptions(Type type)
		{
			IEnumerable<Arg> args;
			if (!_optionsCache.TryGetValue(type, out args))
			{
				args = from i in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					   let arg = GetArgAttribute(i)
					   let argset = GetArgSetAttribute(i)
					   where arg != null || argset != null
					   select new Arg(i, arg, argset);

				args = args.ToList();
				_optionsCache[type] = args;
			}
			return args;
		}

		private static ArgSetAttribute GetArgSetAttribute(PropertyInfo i)
		{
			return (ArgSetAttribute)i.PropertyType.GetCustomAttributes(typeof(ArgSetAttribute), false).FirstOrDefault();
		}

		private static ArgAttribute GetArgAttribute(PropertyInfo info)
		{
			return (ArgAttribute)info.GetCustomAttributes(typeof(ArgAttribute), true).FirstOrDefault();
		}
	}
}