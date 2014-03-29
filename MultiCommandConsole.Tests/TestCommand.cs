using System;
using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Tests
{
    [ConsoleCommand("test|t", "test console command")]
    public class TestCommand : IConsoleCommand
    {
        [Arg("message|m", "this message will be output to the console", Required = true)]
        public string Message { get; set; }

        [Arg("someswitch|ss", "some switch to test switches")]
        public bool Switch { get; set; }

        private IConsoleWriter _writer;

        public IConsoleWriter Writer { get { return _writer; } }

        public CommandsOptions CommandsOptions { get; set; }

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