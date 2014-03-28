using System;
using System.Collections.Generic;
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

	    private void PrintHelp4CommandList()
        {
            var writer = UserInteractiveOptions.Writer;

	        writer.WriteLines(
	            " - type '{command} --help' to see the help for a given command",
	            "Commands:",
                ""
	            );

	        writer.WriteTable(
	            null,
	            _commands.OrderBy(c => c.Attribute.FirstPrototype).Select(ToTableRow),
	            _tableFormat);
        }

	    private void PrintHelp4SingleCommand()
        {
            var writer = UserInteractiveOptions.Writer;

	        writer.WriteLines(
	           _command.Attribute.FirstPrototype + " " + FormatShortNames(_command.Attribute.PrototypeArray.Skip(1)),
	            _instance.GetDetailedHelp() ?? string.Empty);

	        writer.WriteTable(
	            null,
	            ToArgHelpInfo(_command.CommandType, _instance).Distinct().Select(ToTableRow),
	            _tableFormat);
        }

        private static string[] ToTableRow(ConsoleCommandInfo c)
        {
            return new[]
	            {
	                c.Attribute.FirstPrototype,
	                c.Attribute.PrototypeArray.Length > 1
	                    ? FormatShortNames(c.Attribute.PrototypeArray) + c.Attribute.Descripion
	                    : c.Attribute.Descripion
	            };
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