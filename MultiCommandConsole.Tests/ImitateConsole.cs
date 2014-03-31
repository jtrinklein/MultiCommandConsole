using System;
using MultiCommandConsole.Util;
using NUnit.Framework;

namespace MultiCommandConsole.Tests
{
    [TestFixture, Explicit]
	public class ImitateConsole
    {
        private static readonly IConsoleWriter Writer = ConsoleWriter.Get<ImitateConsole>();

		private void Run(string consoleInput)
		{
			Writer.WriteLine("*********************************************");
            Writer.WriteLine("***** Running: " + consoleInput);
            Writer.WriteLine("*********************************************");
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
}
