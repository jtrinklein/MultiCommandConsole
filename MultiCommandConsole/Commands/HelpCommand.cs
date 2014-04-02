using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Commands
{
	[ConsoleCommand(Prototype, "show this message and exit")]
	internal class HelpCommand : IConsoleCommand
	{
		public const string Prototype = "help|?|h";

        private static readonly IConsoleWriter Writer = ConsoleWriter.Get<ConsoleCommandRepository>();

		private IEnumerable<ConsoleCommandInfo> _commands;
		private ConsoleCommandInfo _command;
		private IConsoleCommand _instance;

	    private readonly TableFormat _tableFormat = new TableFormat {Widths = new[] {-1, -1}};

	    public UserInteractiveOptions UserInteractiveOptions { get; set; }

		public static HelpCommand ForCommands(IEnumerable<ConsoleCommandInfo> commands)
		{
			if (commands == null) throw new ArgumentNullException("commands");
			return new HelpCommand {_commands = commands};
		}

		public static HelpCommand ForCommand(ConsoleCommandInfo command, IConsoleCommand instance)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (instance == null) throw new ArgumentNullException("instance");
			return new HelpCommand { _command = command, _instance = instance };
		}

	    public HelpCommand()
	    {
            UserInteractiveOptions = new UserInteractiveOptions();
	    }

	    public string GetDetailedHelp()
		{
			return "\"help\" will display the list of commands available in this console application. " 
				+ Environment.NewLine
				+ "\"help commandName\" or \"commandName /help\" will display help for the given command.";
		}

		public List<string> ExtraArgs { get; set; }

		public IEnumerable<string> GetArgValidationErrors()
		{
			return Enumerable.Empty<string>();
		}

		public void Run()
		{
			if (_command != null)
			{
			    PrintHelp4SingleCommand();
			}
			else
			{
			    PrintHelp4CommandList();
			}
		}

        private class CommandWithCategory
        {
            public string Category { get; set; }
            public ConsoleCommandInfo CommandInfo { get; set; }
        }

	    private void PrintHelp4CommandList()
        {
	        Writer.WriteLines(
	            " - type '{command} --help' to see the help for a given command",
	            "Commands:",
                ""
	            );

	        Writer.WriteTable(
	            null,
	            _commands.Select(c => new CommandWithCategory {CommandInfo = c, Category = GetCategory(c)})
	                     .GroupBy(c => c.Category)
	                     .OrderBy(c => c.Key)
	                     .SelectMany(g =>
	                                 CategoryToTableRows(g.Key)
	                                     .Union(
	                                         CommandsToTableRows(g))),
	            _tableFormat);
        }

        private static string GetCategory(ConsoleCommandInfo c)
        {
            return Config.Help.GetCategoryDelegate == null
                ? null
                : Config.Help.GetCategoryDelegate(c.Attribute.FirstPrototype, c.CommandType);
        }

        private static IEnumerable<string[]> CategoryToTableRows(string category)
        {
            return category.IsNullOrEmpty()
                       ? Enumerable.Empty<string[]>()
                       : new[]
                           {
                               new[] {""},
                               new[] {category}
                           };
        }

	    private static IEnumerable<string[]> CommandsToTableRows(IEnumerable<CommandWithCategory> commands)
	    {
	        return commands.Select(c => c.CommandInfo)
	                       .OrderBy(c => c.Attribute.FirstPrototype)
	                       .Select(CommandToTableRow);
	    }

        private static string[] CommandToTableRow(ConsoleCommandInfo c)
        {
            var helpPrefix = c.Attribute.PrototypeArray.Length > 1
                                 ? FormatShortNames(c.Attribute.PrototypeArray)
                                 : null;

            var addHelpLines = Config.Help.GetAddlHelpDelegate == null
                                   ? null
                                   : Environment.NewLine +
                                   string.Join(Environment.NewLine,
                                                 Config.Help.GetAddlHelpDelegate(c.Attribute.FirstPrototype, c.CommandType));

            return new[]
                {
                    c.Attribute.FirstPrototype,
                    helpPrefix + c.Attribute.Descripion + addHelpLines
                };
        }

        private void PrintHelp4SingleCommand()
        {
            Writer.WriteLines(
               _command.Attribute.FirstPrototype + " " + FormatShortNames(_command.Attribute.PrototypeArray.Skip(1)),
                _instance.GetDetailedHelp() ?? string.Empty);

            Writer.WriteTable(
                null,
                ToArgHelpInfo(_command.CommandType, _instance).Distinct().Select(ToTableRow),
                _tableFormat);
        }

	    private string[] ToTableRow(ArgHelpInfo arg)
        {
            var sb = new StringBuilder();
            if (!arg.OtherPrototypes.IsNullOrEmpty())
            {
                sb.Append(FormatShortNames(arg.OtherPrototypes));
            }
            if (!arg.Description.IsNullOrEmpty())
            {
                sb.AppendLine(arg.Description);
            }
            sb.Append("default=");
            sb.AppendLine(arg.DefaultValue == null ? "{null}" : arg.DefaultValue.ToString());

            return new[]
                {
                    arg.IsRequired ? arg.FirstPrototype + "*" : arg.FirstPrototype,
                    sb.ToString()
                };
        }

	    private static string FormatShortNames(IEnumerable<string> otherPrototypes)
	    {
	        var names = otherPrototypes.ToArray();
            if (names.IsNullOrEmpty())
            {
                return null;
            }
	        return "(" + string.Join(",", names) + ") ";
	    }

	    private class ArgHelpInfo : IComparable<ArgHelpInfo>
        {
            public string FirstPrototype { get; set; }
            public IEnumerable<string> OtherPrototypes { get; set; }
            public bool IsRequired { get; set; }
            public string Description { get; set; }
            public object DefaultValue { get; set; }

            public int CompareTo(ArgHelpInfo other)
            {
                return String.Compare(FirstPrototype, other.FirstPrototype, StringComparison.Ordinal);
            }
        }

	    private IEnumerable<ArgHelpInfo> ToArgHelpInfo(Type type, object instance)
		{
			var options = ArgsHelper.GetOptions(type).ToList();
	        var args = options.Where(o => o.ArgAttribute != null);
	        var argSets = options.Where(p => p.ArgSetAttribute != null);
	        return args.Select(o => ToArgHelpInfo(o, instance))
	                   .Union(argSets.SelectMany(o => ParseArgSet(o.PropertyInfo, instance)));
		}

	    private IEnumerable<ArgHelpInfo> ParseArgSet(PropertyInfo property, object instance)
	    {
	        return ToArgHelpInfo(property.PropertyType, property.GetOrResolve(instance));
	    }

	    private static ArgHelpInfo ToArgHelpInfo(Arg option, object instance)
	    {
	        return new ArgHelpInfo
	            {
	                FirstPrototype = option.ArgAttribute.FirstPrototype,
	                OtherPrototypes = option.ArgAttribute.PrototypeArray.Skip(1),
	                Description = option.ArgAttribute.Description,
	                IsRequired = option.ArgAttribute.Required && option.PropertyInfo.GetValue(instance, null) == option.PropertyInfo.PropertyType.Default(),
	                DefaultValue = option.PropertyInfo.GetValue(instance, null)
	            };
	    }
	}
}