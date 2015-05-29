using System;
using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Commands
{
    [ArgSet("user-interactive", "options for outputting text to console, and ensuring app is in UserInteractive mode")]
    public class UserInteractiveOptions : IValidatable
    {
        private static readonly IConsoleWriter Writer = ConsoleWriter.Get<UserInteractiveOptions>();

        public IEnumerable<string> GetArgValidationErrors()
        {
            if (!MCCEnvironment.UserInteractive)
            {
                return new[] { "console command is only available when the app is run in the console" };
            }
            return Enumerable.Empty<String>();
        }

        public string Prompt(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentNullException("prompt");
            }

            Writer.WriteLine(prompt);
            return Console.ReadLine();
        }
    }
}
