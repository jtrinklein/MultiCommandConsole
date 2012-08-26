using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Commands
{
	[ConsoleCommand(CommandName, "enter into console mode where commands can be typed interactively without exiting the console app.")]
	internal class ConsoleCommand : IConsoleCommand
	{
		readonly Engine _engine;
		readonly ConsoleCommandRepository _consoleCommandRepository;

		public ConsoleCommand(Engine engine, ConsoleCommandRepository consoleCommandRepository)
		{
			_engine = engine;
			_consoleCommandRepository = consoleCommandRepository;
		}

		public const string CommandName = "console";
		
		public string GetDetailedHelp()
		{
			return "\"console\" enter into console mode where commands can be typed interactively without exiting the console app. ";
		}

		public List<string> ExtraArgs { get; set; }

		public IEnumerable<string> GetArgValidationErrors()
		{
			return Enumerable.Empty<String>();
		}

		public void Run()
		{
			using (_consoleCommandRepository.HideConsoleCommand())
			{
				var chunker = Config.ConsoleFormatter;
				chunker.ChunckStringTo(
					"Welcome to console mode.  You will no longer need to type the path & name of the executable.  "
					+ "Enter commands directly at your leisure."
					+ Environment.NewLine
					+ "Type \"quit\" to exit."
					+ Environment.NewLine
					+ "Type \"cls\" to clear the console window."
					+ Environment.NewLine
					+ "Type \"> filename\" to redirect output to a file."
					+ Environment.NewLine,
					Console.Out);

				do
				{
					string[] args;
					do
					{
						Console.Out.WriteLine();
						Console.Out.Write(Config.CommandPromptText);
						args = Console.ReadLine().SplitCmdLineArgs();
					} while (args.IsNullOrEmpty());

					if (args[0].Equals("quit", StringComparison.OrdinalIgnoreCase))
					{
						return;
					}

					if (args[0].Equals("cls", StringComparison.OrdinalIgnoreCase))
					{
						Console.Clear();
					}
					else if (args[0].Equals(CommandName, StringComparison.OrdinalIgnoreCase))
					{
						//already in console mode
					}
					else
					{
						try
						{
							if (args.Length > 2 && args[args.Length - 2] == ">")
							{
								var outputFile = args.Last();
								using(var streamWriter = File.CreateText(outputFile))
								{
									var origConsoleOut = Console.Out;
									using (new DisposableAction(() => Console.SetOut(origConsoleOut)))
									{
										args = args.Take(args.Length - 2).ToArray();
										Console.SetOut(streamWriter);
										_engine.Run(args);
									}
								}
							}
							else
							{
								_engine.Run(args);
							}
						}
						catch
						{
							//Engine already printed to logs and console
						}
					}

				} while (true);
			}
		}
	}
}