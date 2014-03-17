using FluentAssertions;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
    [TestFixture]
    public class ConsoleCommandRepositoryTests
    {
        private ConsoleCommandRepository _repo;

        [SetUp]
        public void SetUp()
        {
            var commands = new[] {typeof (TestCommand)};
            _repo = new ConsoleCommandRepository(null);
            _repo.AddCommands(commands);
        }

        [TearDown]
        public void TearDown()
        {
            Config.DefaultCommand = typeof(HelpCommand);
        }

        [Test]
        public void LoadCommand_Given_DefaultHelpCommand_NoArgs()
        {
            var runData = _repo.LoadCommand(new string[0]);

            runData.Command.Should().BeOfType<HelpCommand>();
        }

        [Test]
        public void LoadCommand_Given_DefaultTestCommand_NoArgs()
        {
            Config.DefaultCommand = typeof(TestCommand);
            var runData = _repo.LoadCommand(new string[0]);

            runData.Command.Should().BeOfType<TestCommand>();
            var testCommand = (TestCommand)runData.Command;
            testCommand.Message.Should().Be(TestCommand.DefaultMessage);
            testCommand.ExtraArgs.Should().BeEmpty();
        }

        [Test]
        public void LoadCommand_Given_DefaultTestCommand_WithArgs()
        {
            Config.DefaultCommand = typeof(TestCommand);
            var runData = _repo.LoadCommand("/m=lala /extra".SplitCmdLineArgs());

            runData.Command.Should().BeOfType<TestCommand>();
            var testCommand = (TestCommand)runData.Command;
            testCommand.Message.Should().Be("lala");
            testCommand.ExtraArgs.Should().BeEquivalentTo("/extra".SplitCmdLineArgs());
        }

        [Test]
        public void LoadCommand_Given_HelpCommand_WithSlash_WithTestArg()
        {
            var runData = _repo.LoadCommand("/help test".SplitCmdLineArgs());

            runData.Command.Should().BeOfType<HelpCommand>();
        }

        [Test]
        public void LoadCommand_Given_HelpCommand_WithoutSlash_WithTestArg()
        {
            var runData = _repo.LoadCommand("help test".SplitCmdLineArgs());

            runData.Command.Should().BeOfType<HelpCommand>();
        }

        [Test]
        public void Should_populate_commands_on_CommandOptions()
        {
            var command = (TestCommand)_repo.LoadCommand("test".SplitCmdLineArgs()).Command;
            command.CommandsOptions.Commands.Should().NotBeEmpty();
            command.CommandsOptions.Commands.Should().BeEquivalentTo(_repo.Commands);
        }
    }
}