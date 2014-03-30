using System;
using System.Linq;
using System.Reflection;
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

        public static object GetOrResolve(this PropertyInfo property, object instance)
        {
            return property.GetValue(instance, null) ?? property.PropertyType.Resolve();
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

        /// <summary>
        /// Sets the first property it finds or the first field if no property was found.
        /// It is assumed their is no value to defining multiple fields or properties of this type
        /// and that when both a field and property exist, the field is a backing field for the property.
        /// </summary>
        /// <typeparam name="TService">the type of property or field to set</typeparam>
        /// <param name="host"></param>
        /// <param name="value"></param>
        public static void SetServiceOnPropertyOrField<TService>(this object host, TService value) 
            where TService: class 
        {
            var writerProp = host.GetType()
                                 .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .FirstOrDefault(p => p.CanWrite && typeof(TService).IsAssignableFrom(p.PropertyType));

            if (writerProp != null)
            {
                if (writerProp.GetValue(host, null) == null)
                {
                    writerProp.SetValue(host, value, null);
                }
                return;
            }

            var writerField = host.GetType()
                                 .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .FirstOrDefault(f => typeof(TService).IsAssignableFrom(f.FieldType));

            if (writerField != null)
            {
                if (writerField.GetValue(host) == null)
                {
                    writerField.SetValue(host, value);
                }
            }
        }
	}
}