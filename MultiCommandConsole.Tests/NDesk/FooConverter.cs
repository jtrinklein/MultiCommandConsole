using System;
using System.ComponentModel;
using System.Globalization;

namespace Tests.Mono.Options
{
	class FooConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof (string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context,
		                                   CultureInfo culture, object value)
		{
			string v = value as string;
			if (v != null)
			{
				switch (v)
				{
					case "A":
						return Foo.A;
					case "B":
						return Foo.B;
				}
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}