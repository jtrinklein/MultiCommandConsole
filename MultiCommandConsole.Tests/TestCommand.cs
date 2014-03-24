using System;
using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Commands;

namespace MultiCommandConsole.Tests
{
    [ConsoleCommand("test|t", "test console command")]
    public class TestCommand : IConsoleCommand
    {
        public static string DefaultMessage = "TestCommand.Run";

        [Arg("message|m", "this message will be output to the console")]
        public string Message { get; set; }

        public CommandsOptions CommandsOptions { get; set; }

        public TestCommand()
        {
            Message = DefaultMessage;
        }

        public string GetDetailedHelp()
        {
            return "Detailed help";
        }

        public List<string> ExtraArgs { get; set; }

        public IEnumerable<string> GetArgValidationErrors()
        {
            return Enumerable.Empty<string>();
        }

        public void Run()
        {
            Console.Out.WriteLine(Message);
        }
    }
}