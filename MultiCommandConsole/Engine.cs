using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using ObjectPrinter;

namespace MultiCommandConsole
{
    public class Engine : ICommandRunner
    {
		private static readonly ILog Log = LogManager.GetLogger<Engine>();

		private readonly ConsoleCommandRepository _commandRepository;

		/// <summary>The name of the application.  Will be used to store console history.</summary>
		public string AppName { get; set; }

		/// <summary>
		/// <para>
		/// A list of types the the Engine may have to resolve to run any of the commands 
		/// from the assemblies and types given in the ctor.
        /// </para>
		/// <para>
		/// Iterate through this list to wire up your IoC container.
        /// </para>
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

			if (Config.RunCommand == null)
			{
				RunImpl(args);
			}
			else
			{
				Config.RunCommand(args, RunImpl);
			}
		}

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void RunImpl(string[] args)
		{
			CommandRunData runData = null;
			try
			{
				runData = _commandRepository.LoadCommand(args);

				runData.SetterUppers.ForEach(su => su.Setup());

				try
				{
					runData.Command.Run();
				}
				finally
				{
					runData.SetterUppers.Reverse();
					runData.SetterUppers.ForEach(su =>
						{
							try
							{
								su.Cleanup();
							}
							catch (Exception e)
							{
								e.SetContext("cleaner upper", su);
								Log.ErrorFormat("failed cleanup for {0}", e, su.GetType().Name);
							}
						});
				}
			}
			catch (TargetInvocationException e)
			{
				if (runData != null && runData.Command != null)
				{
					e.SetContext("command", runData.Command);
				}
				var error = (e.InnerException ?? e).DumpToString();
				Log.Error(error);
				throw;
			}
			catch (Exception e)
			{
				if (runData != null && runData.Command != null)
				{
					e.SetContext("command", runData.Command);
				}
				var error = e.DumpToString();
				Log.Error(error);
				throw;
			}
		}
	}
}