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

            _cmd = new InstallServiceCommand(_svcRepo.Object)
            {
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
    }
}