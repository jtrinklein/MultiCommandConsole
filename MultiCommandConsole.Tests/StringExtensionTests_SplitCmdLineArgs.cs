using FluentAssertions;
using MultiCommandConsole.Util;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
	[TestFixture]
	public class StringExtensionTests_SplitCmdLineArgs
	{
		[Test]
		public void escape_char_is_included_when_not_preceding_a_double_quote()
		{
			"some \\string args".SplitCmdLineArgs().Should().Equal(new[] {"some", "\\string", "args"});
		}

		[Test]
		public void escape_char_is_not_included_when_preceding_a_double_quote()
		{
			"some \\\"string args".SplitCmdLineArgs().Should().Equal(new[] { "some", "\"string", "args" });
		}

		[Test]
		public void spaces_in_single_quotes_should_be_split()
		{
			"'some string' args".SplitCmdLineArgs().Should().Equal(new[] { "'some", "string'", "args" });
		}

		[Test]
		public void spaces_in_double_quotes_should_not_be_split()
		{
			"\"some string\" args".SplitCmdLineArgs().Should().Equal(new[] { "some string", "args" });
		}

		[Test]
		public void spaces_in_escaped_double_quotes_should_be_split()
		{
			"\\\"some string\\\" args".SplitCmdLineArgs().Should().Equal(new[] { "\"some", "string\"", "args" });
		}

		[Test]
		public void single_quotes_and_escaped_double_quotes_should_not_affect_double_quotes()
		{
			"\"\\\"some 'string\\\"\" args'".SplitCmdLineArgs().Should().Equal(new[] { "\"some 'string\"", "args'" });
		}
	}
}