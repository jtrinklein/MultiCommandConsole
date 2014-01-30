using System;
using Mono.Options;

namespace Tests.Mono.Options
{
	class DefaultOption : Option {
		public DefaultOption (string prototypes, string description)
			: base (prototypes, description)
		{
		}

		public DefaultOption (string prototypes, string description, int c)
			: base (prototypes, description, c)
		{
		}

		protected override void OnParseComplete (OptionContext c)
		{
			throw new NotImplementedException ();
		}
	}
}