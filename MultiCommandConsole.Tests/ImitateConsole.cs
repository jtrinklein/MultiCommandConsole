using System;
using System.Collections.Generic;
using System.Linq;
using MultiCommandConsole.Util;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
	[TestFixture, Explicit]
	public class ImitateConsole
	{
		private void Run(string consoleInput)
		{
			Console.Out.WriteLine("*********************************************");
			Console.Out.WriteLine("***** Running: " + consoleInput);
			Console.Out.WriteLine("*********************************************");
			new Engine(new []{typeof(TestCommand),typeof(UncFileCommand)}).Run(consoleInput.SplitCmdLineArgs());
		}
		
		[Test]
		public void Help()
		{
			Run("");
			Run("/?");
			Run("test /?");
			Run("? test");
		}
		
		[Test]
		public void UncPathArgParsing()
		{
			Run(@"uncfile /file=\\server\folder\subfolder\filename.test");
			Run(@"uncfile /file=\\server\C$\diskfolder\filename.xml");
		}
	}

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

	[ConsoleCommand("test|t", "test console command")]
	public class TestCommand : IConsoleCommand
	{
		[Arg("message|m", "this message will be output to the console")]
		public string Message { get; set; }

		public TestCommand()
		{
			Message = "TestCommand.Run";
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
