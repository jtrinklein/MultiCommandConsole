using FluentAssertions;
using MultiCommandConsole.Util;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
    [TestFixture]
    public class StringExtensionTests_Chunking
    {
        [Test]
        public void GetChunk_should_return_correct_chunks()
        {
            //                    1         2
            //          01234567890123456789012345678
            var text = "this is a string of some size";
            text.GetChunk(10, 0).Should().Be("this is a ");
            text.GetChunk(10, 1).Should().Be("string of ");
            text.GetChunk(10, 2).Should().Be("some size");
            text.GetChunk(10, 3).Should().Be(null);

            text.GetChunk(30, 0).Should().Be(text);
            text.GetChunk(30, 1).Should().Be(null);

            text.GetChunk(0, 0).Should().Be(string.Empty);
        }

        [Test]
        public void PivotChunks_should_split_cells_across_lines()
        {
            var cells = new[]
                {
                    //         1         2         3         4
                    //123456789012345678901234567890123456789012
                    "install-services",
                    "installs the specified command as a service"
                };

            cells.PivotChunks(" - ", new[] { cells[0].Length, cells[1].Length })
                 .Should().Equal(new []
                     {
                         "install-services - installs the specified command as a service"
                     });

            cells.PivotChunks(": ", new[] {10, 20})
                 .Should().Equal(new[]
                     {
                         "install-se: installs the specifi",
                         "rvices    : ed command as a serv",
                         "          : ice                 "
                     });

            cells.PivotChunks(": ", "  ", new[] {cells[0].Length, 20})
                 .Should().Equal(new[]
                     {
                         "install-services: installs the specifi",
                         "                  ed command as a serv",
                         "                  ice                 "
                     });
        }
    }
}