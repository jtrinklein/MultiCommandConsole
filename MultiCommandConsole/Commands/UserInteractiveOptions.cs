using System;
using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Commands
{
    [ArgSet("user-interactive", "options for outputting text to console, and ensuring app is in UserInteractive mode")]
    public class UserInteractiveOptions : IValidatable
    {
        private readonly IConsoleWriter _writer = new ConsoleWriter();

        public IConsoleWriter Writer { get { return _writer; } }

        public IEnumerable<string> GetArgValidationErrors()
        {
            if (!Environment.UserInteractive)
            {
                return new[] { "console command is only available when the app is run in the console" };
            }
            return Enumerable.Empty<String>();
        }
    }
}