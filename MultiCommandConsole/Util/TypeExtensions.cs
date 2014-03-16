using System;
using ObjectPrinter;

namespace MultiCommandConsole.Util
{
    internal static class TypeExtensions
	{
		public static object Default(this Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		public static object Resolve(this Type type)
		{
			try
			{
				return Config.ResolveTypeDelegate != null 
					? Config.ResolveTypeDelegate(type) 
					: Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				ex.SetContext("type", type);
				throw;
			}
		}
	}
}