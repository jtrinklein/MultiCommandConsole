using System;
using System.ComponentModel;

namespace MultiCommandConsole.Util
{
	internal static class Converter
	{
		public static T ChangeType<T>(object value)
		{
			return (T)ChangeType(typeof(T), value);
		}

		public static bool TryChangeType<T>(object value, out T result)
		{
			try
			{
				result = ChangeType<T>(value);
				return true;

			}
			catch (Exception)
			{
				result = default(T);
				return false;
			}
		}

		public static bool TryChangeType(Type type, object value, out object result)
		{
			try
			{
				result = ChangeType(type, value);
				return true;

			}
			catch (Exception)
			{
				result = null;
				return false;
			}
		}

		public static object ChangeType(Type type, object value)
		{
			//short circuit
			if (value == null || value.GetType().IsAssignableFrom(type))
			{
				return value;
			}

			if (value is string)
			{
				// special case handling for parsing System.Type, assuming value is a type name
				if (typeof(Type).IsAssignableFrom(type))
				{
					try
					{
						return Type.GetType(value as string, true, true);
					}
					catch (Exception)
					{
						//obviously not an AssemblyQualifiedName.  Let the default type converter have a shot at it.
					}
				}
			}

			TypeConverter tc = TypeDescriptor.GetConverter(type);
			if (tc == null)
			{
				return null;
			}

			return tc.ConvertFrom(value);
		}

		public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
		{
			TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
		}
	}
}