using System;
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
			new Engine(new []{typeof(TestCommand),typeof(UncFileCommand)}).GetRunner().Run(consoleInput.SplitCmdLineArgs());
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
