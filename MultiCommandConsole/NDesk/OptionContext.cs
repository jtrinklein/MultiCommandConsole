namespace Mono.Options
{
	public class OptionContext
	{
		public Option Option { get; set; }
		public string OptionName { get; set; }
		public int OptionIndex { get; set; }
		public OptionSet OptionSet { get; private set; }
		public OptionValueCollection OptionValues { get; private set; }

		public OptionContext(OptionSet set)
		{
			this.OptionSet = set;
			this.OptionValues = new OptionValueCollection(this);
		}
	}
}