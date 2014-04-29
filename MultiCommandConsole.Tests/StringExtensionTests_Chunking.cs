using System.Linq;
using FluentAssertions;
using MultiCommandConsole.Util;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
    [TestFixture]
    public class StringExtensionTests_Chunking
    {
        [Test]
        public void WholeWordSubstring_should_break_on_whitepace_and_punctuation()
        {
            //                   1         2         3         4
            //          123456789012345678901234567890123456789012
            var text = "installs the specified command-as a service";

            int endIndex;
            text.WholeWordSubstring(0, 21, out endIndex).Should().Be("installs the");
            text.WholeWordSubstring(0, 22, out endIndex).Should().Be("installs the specified");
            text.WholeWordSubstring(0, 30, out endIndex).Should().Be("installs the specified command");
            text.WholeWordSubstring(0, 33, out endIndex).Should().Be("installs the specified command-as");
        }

        [Test]
        public void WholeWordSubstring_should_trim_starting_whitepace()
        {
            //                        1         2         3         4
            //               123456789012345678901234567890123456789012
            var text = " \t\ninstalls stuff";
            int endIndex;
            text.WholeWordSubstring(0, 8, out endIndex).Should().Be("installs");
            text.WholeWordSubstring(0, 14, out endIndex).Should().Be("installs stuff");
        }

        [Test]
        public void WholeWordSubstring_should_return_entire_text_when_length_permits()
        {
            var text = "install-services";
            int endIndex;
            text.WholeWordSubstring(0, 16, out endIndex).Should().Be(text);
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
                         " install-services - installs the specified command as a service"
                     });

            cells.PivotChunks(": ", new[] {10, 20})
                 .Should().Equal(new[]
                     {
                         " install-  : installs the        ",
                         " services  : specified command as",
                         "           : a service           "
                     });

            cells.PivotChunks(": ", "  ", new[] {cells[0].Length, 20})
                 .Should().Equal(new[]
                     {
                         " install-services: installs the        ",
                         "                   specified command as",
                         "                   a service           "
                     });
        }

        [Test]
        public void GetChunk_should_handle_null()
        {
            ((string) null).GetChunks(10).Should().BeEmpty();
        }

        [Test]
        public void GetChunk_should_handle_empty()
        {
            string.Empty.GetChunks(10).Should().Equal(new []{string.Empty});
        }

        [Test]
        public void GetChunks_should_pad_right()
        {
            //         1         2         3         4
            //123456789012345678901234567890123456789012
            "installs the specified command as a service"
                .GetChunks(20).Should().Equal(new object[]
                    {
                        "installs the",
                        "specified command as",
                        "a service"
                    });
        }

        [Test]
        public void GetChunk_should_handle_text_shorter_than_chunksize()
        {
            var text = "short text";
            text.GetChunks(10).Should().Equal(new []{text});
            text.GetChunks(9).Should().Equal(new[] { "short", "text" });
        }

        [Test]
        public void GetChunk_should_account_for_newlines()
        {
            //             012345678901234567890
            var chunks1 = "this textdoes not".GetChunks(13).ToList();
            var chunks2 = "this text\n  wraps".GetChunks(13).ToList();
            chunks1.Should().Equal(new []
                {
                    "this textdoes",
                    "not"
                });
            chunks2.Should().Equal(new[]
                {
                    "this text",
                    "  wraps"
                });
        }

        [Test]
        public void WholeWordSubstring_should_handle_no_breaks()
        {
            "0123456789".GetChunks(5).Should().Equal(new[] { "01234", "56789" });
            "123456789".GetChunks(3).Should().Equal(new[] { "123", "456", "789" });
            "1234567890".GetChunks(4).Should().Equal(new[] { "1234", "5678", "90" });
        }
    }
}