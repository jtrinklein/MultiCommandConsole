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

        /// <summary>
        /// if the obj is type of T, then performs action on obj as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns>true if obj is of type T, else false</returns>
        public static bool As<T>(this object obj, Action<T> action) where T : class
        {
            var o = obj as T;
            if (o != null)
            {
                action(o);
                return true;
            }
            return false;
        }
	}
}