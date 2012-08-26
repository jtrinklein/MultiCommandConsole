using System.Linq;
using FluentAssertions;
using MultiCommandConsole.Util;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
	[TestFixture]
	public class ConsoleFormatterTests
	{
		[Test]
		public void ChuckString_should_honor_ChunkSize()
		{
			var chunker = new ConsoleFormatter {OverriddenBufferWidth = 2};
			var chunks = chunker.ChunkString("0123456789").ToList();
			chunks.Count.Should().Be(5);
			chunks.ForEach(c => c.Length.Should().Be(2));

			chunker = new ConsoleFormatter {OverriddenBufferWidth = 3};
			chunks = chunker.ChunkString("123456789").ToList();
			chunks.Count.Should().Be(3);
			chunks.ForEach(c => c.Length.Should().Be(3));
		}

		[Test]
		public void ChuckString_should_honor_decreaseChunkBy()
		{
			var chunker = new ConsoleFormatter {OverriddenBufferWidth = 4};
			var chunks = chunker.ChunkString("0123456789", decreaseChunkBy: 2).ToList();
			chunks.Count.Should().Be(5);
			chunks.ForEach(c => c.Length.Should().Be(2));

			chunker = new ConsoleFormatter { OverriddenBufferWidth = 8 };
			chunks = chunker.ChunkString("123456789", decreaseChunkBy: 5).ToList();
			chunks.Count.Should().Be(3);
			chunks.ForEach(c => c.Length.Should().Be(3));
		}
	}
}