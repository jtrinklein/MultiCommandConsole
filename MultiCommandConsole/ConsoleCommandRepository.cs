using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Mono.Options;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole
{
	internal class ConsoleCommandRepository
	{
	    private static readonly ILog Log = LogManager.GetLogger<ConsoleCommandRepository>();

        internal readonly Dictionary<string, ConsoleCommandInfo> CommandsByName = new Dictionary<string, ConsoleCommandInfo>(StringComparer.OrdinalIgnoreCase);
        internal readonly Dictionary<Type, ConsoleCommandInfo> CommandsByType = new Dictionary<Type, ConsoleCommandInfo>();
        internal readonly IConsoleFormatter Chunker = Config.ConsoleFormatter;

		internal ConsoleCommand ConsoleCommand { get; set; }

		private IEnumerable<ConsoleCommandInfo> _commands;
		public IEnumerable<ConsoleCommandInfo> Commands
		{
			get
			{
				if (_commands == null)
				{
					var commands = from c in CommandsByType.Values
					            let isInternal = c.CommandType.Assembly == GetType().Assembly
					            orderby isInternal descending , c.Attribute.Prototype
					            select c;

					_commands = commands.ToList();
				}
				return _commands;
			}
		}

		public ConsoleCommandRepository()
        {
		    //these commands should never be created by the ResolveTypeDelegate.  They're internal only
			AddCommand(BuildCommandInfo(new HelpCommand()));
			if (Config.ConsoleMode.Enabled)
			{
				AddCommand(BuildCommandInfo(ConsoleCommand = new ConsoleCommand()));
			}
			if (Config.ShowVierArgsCommand)
			{
				AddCommand(BuildCommandInfo(new ViewArgsCommand()));
			}
		}

		public void AddCommands(IEnumerable<Assembly> assemblies)
		{
			AddCommands(assemblies.SelectMany(a => a.GetTypes()));
		}

		public void AddCommands(IEnumerable<Type> types)
		{
			var commands = types
				.Where(t => t.IsClass
				            && !t.IsAbstract
				            && typeof (IConsoleCommand).IsAssignableFrom(t)
				            && t.Namespace != typeof (HelpCommand).Namespace)
				.Select(BuildCommandInfo)
				.Where(info => info != null);

			foreach (var info in commands)
			{
				AddCommand(info);
			}
		}

		private void AddCommand(ConsoleCommandInfo info)
		{
            CommandsByType.Add(info.CommandType, info);
			foreach (var name in info.Attribute.Prototype.GetPrototypeArray())
			{
				CommandsByName.Add(name, info);
			}
		}

		static ConsoleCommandInfo BuildCommandInfo(IConsoleCommand command)
		{
			var info = BuildCommandInfo(command.GetType());
			info.Instance = command;
			return info;
		}

		static ConsoleCommandInfo BuildCommandInfo(Type type)
		{
			var attr = type
				.GetCustomAttributes(typeof (ConsoleCommandAttribute), false)
				.Cast<ConsoleCommandAttribute>()
				.FirstOrDefault();

			if (attr == null)
			{
				Log.DebugFormat("Skipping {0} because it doesn't have a ConsoleCommandAttribute", type.Name);
				return null;
			}

			return new ConsoleCommandInfo
			       	{
			       		Attribute = attr,
			       		CommandType = type
			       	};
		}

	    public ConsoleCommandInfo GetByPrototype(string name)
        {
            ConsoleCommandInfo info;
            return CommandsByName.TryGetValue(name, out info) ? info : null;
	    }

	    public ConsoleCommandInfo GetByType(Type type)
        {
            ConsoleCommandInfo info;
            return CommandsByType.TryGetValue(type, out info) ? info : null;
	    }

        internal DisposableAction HideCommandOfType<T>() where T : IConsoleCommand
        {
            var commandType = typeof(T);

            var restoreActions = new List<Action>();

            ConsoleCommandInfo commandInfo;
            if (CommandsByType.TryGetValue(commandType, out commandInfo))
            {
                ConsoleCommandInfo info = commandInfo;  //access to modified closure

                CommandsByType.Remove(commandType);
                restoreActions.Add(() => CommandsByType.Add(commandType, info));

                restoreActions.AddRange(commandInfo.Attribute.Prototype
                                                   .GetPrototypeArray()
                                                   .Where(n => CommandsByName.Remove(n))
                                                   .Select(name => (Action) (() => CommandsByName.Add(name, info))));
            }

            return new DisposableAction(() => restoreActions.ForEach(action => action()));
		}

		public IEnumerable<ConsoleCommandAttribute> GetCommandList()
		{
			return Commands.Select(v => v.Attribute);
		}

		public CommandRunData LoadCommand(string[] args)
        {
            var optionPrefixes = new[] { '/', '-' };
			ConsoleCommandInfo info = null;

			if(args.IsNullOrEmpty() || string.IsNullOrEmpty(args[0]))
            {
                CommandsByType.TryGetValue(Config.DefaultCommand, out info);
                if (info != null)
                {
                    args = new[] {info.Attribute.Prototype.GetPrototypeArray().First()}.Union(args).ToArray();
                }
            }

			var commandName = args[0].Trim();
            if (optionPrefixes.Any(p => p == commandName[0]))
            {
                //did the user type /help first?
                if (CommandsByName.TryGetValue(commandName.Substring(1), out info)
                    && info == CommandsByType[typeof(HelpCommand)])
                {
                    commandName = commandName.TrimStart(optionPrefixes);
                }
                else
                {
                    CommandsByType.TryGetValue(Config.DefaultCommand, out info);
                    if (info != null)
                    {
                        args = new[] { info.Attribute.Prototype.GetPrototypeArray().First() }.Union(args).ToArray();
                    }
                }
            }

			bool showHelp = false;
			var options = new OptionSet();

            if (info != null || CommandsByName.TryGetValue(commandName, out info))
			{
				if (info.CommandType == typeof(HelpCommand))
				{
					if (args.Length > 1)
					{
						return LoadCommand(new[]{args[1], "/" + commandName});
					}
					return new CommandRunData { Command = HelpCommand.ForCommands(Commands) };
				}

				IConsoleCommand command;
				try
				{
					command = info.Instance ?? (IConsoleCommand)info.CommandType.Resolve();
				}
				catch (Exception e)
				{
					e.SetContext("commandInfo", info);
					throw;
				}

				var validators = new List<IValidatable> { command };
				var setterUppers = new List<ISetupAndCleanup>();
			    var setterUpper = command as ISetupAndCleanup;
				if (setterUpper != null)
				{
					setterUppers.Add(setterUpper);
				}
				LoadArgs(options, validators, setterUppers, command);
				options.Add(HelpCommand.Prototype, "show this message and exit", a => showHelp = true);
				try
				{
					command.ExtraArgs = options.Parse(args.Skip(1));
					if (showHelp)
					{
						return new CommandRunData { Command = HelpCommand.ForCommand(info, command) };
					}

					var errors = validators.OrderBy(v => v is IConsoleCommand).SelectMany(v => v.GetArgValidationErrors()).ToList();
					if (errors.Count > 0)
					{
						foreach (var error in errors)
						{
							Console.Out.Write("!!! ");
							Chunker.ChunckStringTo(error, Console.Out);
						}
						return new CommandRunData { Command = HelpCommand.ForCommand(info, command) };
					}
				}
				catch (Exception)
				{
					return new CommandRunData { Command = HelpCommand.ForCommand(info, command) };
				}

				return new CommandRunData { Command = command, SetterUppers = setterUppers};

			}

			Console.Out.WriteLine("Unknown command: " + commandName);
			return new CommandRunData { Command = HelpCommand.ForCommands(Commands) };
		}

		private void LoadArgs(OptionSet optionSet, List<IValidatable> validators, List<ISetupAndCleanup> setterUppers, object obj)
		{
			var options = ArgsHelper.GetOptions(obj.GetType()).ToList();

			foreach (var option in options.Where(p => p.ArgSetAttribute != null))
			{
				var args = option.PropertyInfo.GetValue(obj, null) ?? option.PropertyInfo.PropertyType.Resolve();

			    args.As<CommandsOptions>(o => { o.ConsoleCommandRepository = this; });
                args.As<ConsoleRunOptions>(o => { o.CreateCommandRunner = () => new CommandRunner(this); });
                args.As<IValidatable>(validators.Add);
                args.As<ISetupAndCleanup>(setterUppers.Add);

				//load all arguments in a set before assigning it to the host property.
				//  this allows the host to act on the fully loaded set when it's assigned.
				LoadArgs(optionSet, validators, setterUppers, args);
				option.PropertyInfo.SetValue(obj, args, null);
			}
		
			foreach (var option in options.Where(p => p.ArgAttribute != null))
			{
				var opt = option.ArgAttribute;
				var defaultValue = option.PropertyInfo.GetValue(obj, null);
				bool isRequired = option.ArgAttribute.Required && defaultValue == option.PropertyInfo.PropertyType.Default();

				var prototype = opt.Prototype + (isRequired ? "=" : ":");
				var description = isRequired 
					? opt.Description
					: string.Format("{0} default={1}", opt.Description, defaultValue);

				var localProperty = option;
				optionSet.Add(prototype, description,
				            s =>
				            	{
				            		var type = localProperty.PropertyInfo.PropertyType;
				            		var value = type == typeof (bool)
				            		            	? true
				            		            	: Converter.ChangeType(type, s);
									localProperty.PropertyInfo.SetValue(obj, value, null);
				            	});
			}
		}
	}
}
