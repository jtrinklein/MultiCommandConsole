using System.Linq;
using FluentAssertions;
using MultiCommandConsole.Commands;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
    [TestFixture]
    public class BurstSegmentTests
    {
        [Test]
        public void ShoudParseSingleSegment()
        {
            var segments = BurstSegment.ParseSegments("s10w5r2").ToList();
            segments.Count.Should().Be(1);
            segments[0].Send.Should().Be(10);
            segments[0].Wait.Should().Be(5);
            segments[0].Repeat.Should().Be(2);
        }

        [Test]
        public void ShoudParseSingleSegmentWithInfiniteRepeat()
        {
            var segments = BurstSegment.ParseSegments("s10w5r-1").ToList();
            segments.Count.Should().Be(1);
            segments[0].Send.Should().Be(10);
            segments[0].Wait.Should().Be(5);
            segments[0].Repeat.Should().Be(-1);
        }
        [Test]
        public void ShoudParseSingleSegmentWithoutRepeat()
        {
            var segments = BurstSegment.ParseSegments("s10w5").ToList();
            segments.Count.Should().Be(1);
            segments[0].Send.Should().Be(10);
            segments[0].Wait.Should().Be(5);
            segments[0].Repeat.Should().Be(1);
        }
        [Test]
        public void ShoudParseSingleSegmentWithoutWait()
        {
            var segments = BurstSegment.ParseSegments("s10r2").ToList();
            segments.Count.Should().Be(1);
            segments[0].Send.Should().Be(10);
            segments[0].Wait.Should().Be(0);
            segments[0].Repeat.Should().Be(2);
        }
        [Test]
        public void ShoudParseSingleSegmentWithoutWaitOrRepeat()
        {
            var segments = BurstSegment.ParseSegments("s10").ToList();
            segments.Count.Should().Be(1);
            segments[0].Send.Should().Be(10);
            segments[0].Wait.Should().Be(0);
            segments[0].Repeat.Should().Be(1);
        }

        [Test]
        public void ShoudParseMultiSegment()
        {
            var segments = BurstSegment.ParseSegments("s10w5r2s20w15r200").ToList();
            segments.Count.Should().Be(2);
            segments[0].Send.Should().Be(10);
            segments[0].Wait.Should().Be(5);
            segments[0].Repeat.Should().Be(2);
            segments[1].Send.Should().Be(20);
            segments[1].Wait.Should().Be(15);
            segments[1].Repeat.Should().Be(200);
        }

        [Test]
        public void ShoudParseMultiSegmentWithoutRepeat()
        {
            var segments = BurstSegment.ParseSegments("s10w5s20w15").ToList();
            segments.Count.Should().Be(2);
            segments[0].Send.Should().Be(10);
            segments[0].Wait.Should().Be(5);
            segments[0].Repeat.Should().Be(1);
            segments[1].Send.Should().Be(20);
            segments[1].Wait.Should().Be(15);
            segments[1].Repeat.Should().Be(1);
        }
    }
}