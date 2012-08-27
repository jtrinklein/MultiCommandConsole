using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiCommandConsole.Util;

namespace MultiCommandConsole
{
	public static class ArgsHelper
	{
		private static Dictionary<Type, IEnumerable<Arg>> _optionsCache = new Dictionary<Type, IEnumerable<Arg>>();
		private static Dictionary<Type, List<string>> _flattenedMetaDataCache = new Dictionary<Type, List<string>>();

		public static IEnumerable<Arg> GetOptions(Type type)
		{
			IEnumerable<Arg> args;
			if (!_optionsCache.TryGetValue(type, out args))
			{
				_optionsCache[type] = args = GetOptionsImpl(type).ToList();
			}
			return args;
		}

		public static List<string> GetFlattenedOptionNames(Type type)
		{
			List<string> data;
			if (!_flattenedMetaDataCache.TryGetValue(type, out data))
			{
				data = GetOptionsImpl(type, true)
					.Select(a => a.ArgAttribute.Prototype.GetPrototypeArray().First())
					.ToList();
				_flattenedMetaDataCache[type] = data;
			}
			return data;
		}

		private static IEnumerable<Arg> GetOptionsImpl(Type type, bool recursive = false)
		{
			var args = from i in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			           let arg = GetArgAttribute(i)
			           let argset = GetArgSetAttribute(i)
			           where arg != null || argset != null
			           select new Arg(i, arg, argset);
			if (!recursive)
			{
				return args;
			}
			var innerArgs = from arg in args
			                where arg.ArgSetAttribute != null
			                select GetOptionsImpl(arg.PropertyInfo.PropertyType, true);
			return args.Where(a => a.ArgAttribute != null).Union(innerArgs.SelectMany(a => a));
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