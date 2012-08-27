using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole
{
	public class Engine
	{
		private static ILogger Log = Logging.GetLogger<Engine>();

		private ConsoleCommandRepository _commandRepository;

		/// <summary>The name of the application.  Will be used to store console history.</summary>
		public string AppName { get; set; }

		/// <summary>The number of entries to keep in history.</summary>
		public int HistorySize { get; set; }

		/// <summary>
		/// A list of types the the Engine may have to resolve to run any of the commands 
		/// from the assemblies and types given in the ctor.
		/// </summary>
		public IEnumerable<Type> TypesToResolve
		{
			get
			{
				var commands = _commandRepository.Commands;
				foreach (var command in commands)
				{
					yield return command.CommandType;
					foreach (var arg in ArgsHelper.GetOptions(command.CommandType).Where(a => a.ArgSetAttribute != null))
					{
						yield return arg.PropertyInfo.PropertyType;
					}
				}
			}
		}

		public Engine(IEnumerable<Type> types)
		{
			_commandRepository = new ConsoleCommandRepository(this);
			_commandRepository.AddCommands(types);
		}

		public Engine(IEnumerable<Assembly> assemblies)
		{
			_commandRepository = new ConsoleCommandRepository(this);
			_commandRepository.AddCommands(assemblies);
		}

		public void Run(string[] args)
		{
			Log.InfoFormat("Running: {0}", string.Join(" ", args));

			try
			{
				var command = _commandRepository.LoadCommand(args);

				if (!(command is HelpCommand))
				{
					Log.DebugFormat("Running command: {0}", command.DumpToString());
				}
				command.Run();
			}
			catch (TargetInvocationException e)
			{
				var error = (e.InnerException ?? e).DumpToString();
				Console.Out.WriteLine(error);
				Console.Out.WriteLine();
				Console.Out.WriteLine("See logs for more details");
				Log.Error(error);
				throw;
			}
			catch (Exception e)
			{
				var error = e.DumpToString();
				Console.Out.WriteLine(error);
				Console.Out.WriteLine();
				Console.Out.WriteLine("See logs for more details");
				Log.Error(error);
				throw;
			}
		}
	}
}