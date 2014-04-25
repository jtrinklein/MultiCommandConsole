using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using FluentAssertions;
using Moq;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Services;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
    [TestFixture]
    public class InstallServiceCommandTests
    {
        private Mock<IConsoleCommandRepository> _cmdRepo;
        private Mock<IServicesRepository> _svcRepo;
        private InstallServiceCommand _cmd;

        [SetUp]
        public void Setup()
        {
            _cmdRepo = new Mock<IConsoleCommandRepository>();
            _svcRepo = new Mock<IServicesRepository>();

            _cmdRepo.Setup(r => r.LoadCommand(It.IsAny<string[]>()))
                    .Returns(new CommandRunData { Command = new TestServiceCommand() });

            _cmd = new InstallServiceCommand(_svcRepo.Object)
            {
                ExtraArgs = new List<string>(),
                CommandsOptions = new CommandsOptions { ConsoleCommandRepository = _cmdRepo.Object },
            };
        }

        [Test]
        public void Validation_WhenNoArgsAreSupplied()
        {
            var errors = _cmd.GetArgValidationErrors().ToList();
            errors.Should().NotBeEmpty();
            errors.Should().Contain("command is required");
        }

        [Test]
        public void Validation_WhenServiceAccountIsUserAndNoUsernameOrPassword()
        {
            _cmd.Account = ServiceAccount.User;
            var errors = _cmd.GetArgValidationErrors().ToList();
            errors.Should().NotBeEmpty();
            errors.Should().Contain("user is required when account=User");
            errors.Should().Contain("pwd is required when account=User");
        }

        [Test]
        public void Validation_ShouldDefaultsServiceInfoFromCommand()
        {
            _cmd.CommandLine = "test";
            var errors = _cmd.GetArgValidationErrors().ToList();
            errors.Should().BeEmpty();
            var testCommand = new TestServiceCommand();
            _cmd.ServiceName.Should().Be(testCommand.ServiceName);
            _cmd.DisplayName.Should().Be(testCommand.DisplayName);
            _cmd.Description.Should().Be(testCommand.Description);
        }

        [Test]
        public void Validation_ShouldPreferUserDefinedServiceInfo()
        {
            _cmd.CommandLine = "test";
            _cmd.ServiceName = "sn1";
            _cmd.DisplayName = "dn1";
            _cmd.Description = "desc1";

            var errors = _cmd.GetArgValidationErrors().ToList();
            errors.Should().BeEmpty();
            _cmd.ServiceName.Should().Be("sn1");
            _cmd.DisplayName.Should().Be("dn1");
            _cmd.Description.Should().Be("desc1");
        }
    }
}