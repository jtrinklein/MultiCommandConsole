using System;
using Mono.Options;

namespace MultiCommandConsole
{
	public class EnumParserOption<T> : Option where T : struct, IConvertible
	{
		readonly Action<T> _action;

		public EnumParserOption(string prototype, string description, Action<T> action)
			: base(prototype, description)
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			_action = action;
		}

		public EnumParserOption(string prototype, string description, int maxValueCount, Action<T> action)
			: base(prototype, description, maxValueCount)
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			_action = action;
		}

		protected override void OnParseComplete(OptionContext c)
		{
			if (c.OptionValues == null)
			{
				return;
			}

			uint value = 0;
			foreach (var optionValue in c.OptionValues)
			{
				uint val;
				if (Enum.TryParse(optionValue, true, out val))
				{
					value = value | val;
				}
			}

			_action((T)Enum.ToObject(typeof(T), value));
		}
	}
}