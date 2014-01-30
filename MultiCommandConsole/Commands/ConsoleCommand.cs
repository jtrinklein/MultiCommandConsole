using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Terminal;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole.Commands
{
	[ConsoleCommand(CommandName, "enter into console mode where commands can be typed interactively without exiting the console app.")]
	internal class ConsoleCommand : IConsoleCommand
	{
		readonly Engine _engine;
		readonly ConsoleCommandRepository _consoleCommandRepository;
		static readonly char[] ArgPrefixes = new[] { '/', '-' };

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

		private Dictionary<string, ConsoleCommandInfo> _commandCache;
		private Dictionary<string, List<string>> _optionCache = new Dictionary<string, List<string>>();

		public void Run()
		{
			var le = new LineEditor(_engine.AppName, _engine.HistorySize == 0 ? 10 : _engine.HistorySize)
			         	{
			         		AutoCompleteEvent = (text, pos) => GetEntries(text)
			         	};

			using (_consoleCommandRepository.HideConsoleCommand())
			{
				var chunker = Config.ConsoleFormatter;
				chunker.ChunckStringTo(
					"Type \"quit\" to exit."
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
						args = le.Edit(Config.CommandPromptText +  "> ", string.Empty).SplitCmdLineArgs();
					} while (args.IsNullOrEmpty());

					if (args[0].Equals("quit", StringComparison.OrdinalIgnoreCase))
					{
						le.SaveHistory();
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
						        using (var streamWriter = File.CreateText(outputFile))
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
						finally
						{
						    le.SaveHistory();
						}
					}

				} while (true);
			}
		}

		private LineEditor.Completion GetEntries(string text)
		{
			string commandName = null;
			string[] parts = null;
			try
			{
				if (_commandCache == null)
				{
					_commandCache = new Dictionary<string, ConsoleCommandInfo>();
					foreach(var command in _consoleCommandRepository.Commands)
					{
						foreach(var name in command.Attribute.Prototype.GetPrototypeArray())
						{
							_commandCache.Add(name, command);
						}
					}
				}

				parts = text.Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

				commandName = parts.FirstOrDefault();

				if (commandName == null)
				{
					return new LineEditor.Completion(string.Empty, new string[0]);
				}

				if (parts.Length == 1)
				{
					//looking for a command name
					var commands = _commandCache.Keys
						.Where(c => c.StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
						.Select(s => s.Substring(commandName.Length) + " ")
						.ToArray();
					return new LineEditor.Completion(commandName, commands);
				}

				var lastPart = parts.Last().TrimStart(ArgPrefixes); //remove the / or - arguments are prefixed for

				List<string> argNames;
				if (!_optionCache.TryGetValue(commandName, out argNames))
				{
					var commandInfo = _commandCache[commandName];
					_optionCache[commandName] = argNames = ArgsHelper.GetFlattenedOptionNames(commandInfo.CommandType);
				}

				var options = argNames
					.Where(a => a.StartsWith(lastPart, StringComparison.OrdinalIgnoreCase))
					.Where(a => !parts.Contains(a))
					.Select(s => s.Substring(lastPart.Length) + "=")
					.ToArray();
				return new LineEditor.Completion(lastPart, options);
			}
			catch (Exception e)
			{
				e.SetContext("text", text);
				e.SetContext("commandName", commandName);
				e.SetContext("parts", parts);
				throw;
			}
		}
	}
}