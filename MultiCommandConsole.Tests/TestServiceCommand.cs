using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Services;

namespace MultiCommandConsole.Tests
{
    [ConsoleCommand("test", "test console command")]
    public class TestServiceCommand : IConsoleCommand, ICanRunAsService
    {
        public string ServiceName { get { return "TestService"; } }
        public string DisplayName { get { return "Test Service"; } }
        public string Description { get { return "A Test Service"; } }

        public IEnumerable<string> GetArgValidationErrors()
        {
            return Enumerable.Empty<string>();
        }

        public string GetDetailedHelp()
        {
            return "";
        }

        public List<string> ExtraArgs { get; set; }

        public void Run()
        {
            /*no op*/
        }

        public void Stop()
        {
            /*no op*/
        }
    }
}