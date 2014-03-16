using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiCommandConsole.Tests
{
    [ConsoleCommand("uncfile", "test unc file paths console command")]
    public class UncFileCommand : IConsoleCommand
    {
        [Arg("file|f", "this message will be output to the console")]
        public string File { get; set; }

        public string GetDetailedHelp()
        {
            return "Detailed help";
        }

        public List<string> ExtraArgs { get; set; }

        public IEnumerable<string> GetArgValidationErrors()
        {
            if(!File.StartsWith(@"\\"))
            {
                return new[] {@"File should start with \\"};
            }
            return Enumerable.Empty<string>();
        }

        public void Run()
        {
            Console.Out.WriteLine(File);
        }
    }
}