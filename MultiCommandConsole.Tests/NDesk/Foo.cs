using System.ComponentModel;

namespace Tests.Mono.Options
{
	[TypeConverter(typeof (FooConverter))]
	class Foo
	{
		public static readonly Foo A = new Foo("A");
		public static readonly Foo B = new Foo("B");
		string s;

		Foo(string s)
		{
			this.s = s;
		}

		public override string ToString()
		{
			return s;
		}
	}
}