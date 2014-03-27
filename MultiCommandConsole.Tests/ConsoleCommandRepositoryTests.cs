using System;
using FluentAssertions;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;
using NUnit.Framework;
using ObjectPrinter;

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
            _repo = new ConsoleCommandRepository();
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
            var command = (TestCommand)_repo.LoadCommand("test /m=msg-is-required".SplitCmdLineArgs()).Command;
            command.CommandsOptions.Commands.Should().NotBeEmpty();
            command.CommandsOptions.Commands.Should().BeEquivalentTo(_repo.Commands);
        }

        [Test]
        public void WhenBoolArgIsSpecifiedWithoutValue_Should_DefaultToTrue()
        {
            var command = (TestCommand)_repo.LoadCommand("test /ss /m=msg-is-required".SplitCmdLineArgs()).Command;
            command.Switch.Should().BeTrue();
        }

        [Test]
        public void WhenBoolArgIsSpecifiedWithTrue_Should_BeTrue()
        {
            var command = (TestCommand)_repo.LoadCommand("test /ss=true /m=msg-is-required".SplitCmdLineArgs()).Command;
            command.Switch.Should().BeTrue();
        }

        [Test]
        public void WhenBoolArgIsSpecifiedWithFalse_Should_BeFalse()
        {
            var command = (TestCommand)_repo.LoadCommand("test /ss=false /m=msg-is-required".SplitCmdLineArgs()).Command;
            command.Switch.Should().BeFalse();
        }

        [Test]
        public void WhenRequiredArgIsSpecified_ShouldNotError()
        {
            var command = _repo.LoadCommand("test /m=msg-is-required".SplitCmdLineArgs());
            command.Errors.Should().BeEmpty();
        }

        [Test]
        public void WhenRequiredArgIsNotSpecified_ShouldError()
        {
            var command = _repo.LoadCommand("test".SplitCmdLineArgs());
            command.Errors.Should().NotBeEmpty();
            command.Errors.Should().Contain("message is required");
        }
    }
}