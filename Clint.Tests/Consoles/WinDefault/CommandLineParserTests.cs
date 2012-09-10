using Clint.Consoles.WinDefault;
using Clint.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace Clint.Tests.Consoles.WinDefault
{
	[TestFixture]
	public class CommandLineParserTests
	{
		CommandLineParser parser = new CommandLineParser();

		[Test]
		public void escape_char_is_included_when_not_preceding_a_double_quote()
		{
			parser.Parse("some \\string args").Should().ContainInOrder(e._("some", "\\string", "args"));
		}

		[Test]
		public void escape_char_is_not_included_when_preceding_a_double_quote()
		{
			parser.Parse("some \\\"string args").Should().ContainInOrder(e._("some", "\"string", "args"));
		}

		[Test]
		public void spaces_in_single_quotes_should_be_split()
		{
			parser.Parse("'some string' args").Should().ContainInOrder(e._("'some", "string'", "args"));
		}

		[Test]
		public void spaces_in_double_quotes_should_not_be_split()
		{
			parser.Parse("\"some string\" args").Should().ContainInOrder(e._("some string", "args"));
		}

		[Test]
		public void spaces_in_escaped_double_quotes_should_be_split()
		{
			parser.Parse("\\\"some string\\\" args").Should().ContainInOrder(e._("\"some", "string\"", "args"));
		}

		[Test]
		public void single_quotes_and_escaped_double_quotes_should_not_affect_double_quotes()
		{
			parser.Parse("\"\\\"some 'string\\\"\" args'").Should().ContainInOrder(e._("\"some 'string\"", "args'"));
		}
	}
}