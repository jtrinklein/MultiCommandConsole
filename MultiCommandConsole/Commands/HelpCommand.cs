using System;
using System.Collections.Generic;
using System.Linq;
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
		ConsoleFormatter _chunker = Config.ConsoleFormatter;

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
				var section = new Section
				{
					Header = new[]
				              	{
				              		BuildDisplayName(string.Empty, _command.Attribute.Prototype),
									_instance.GetDetailedHelp() ?? string.Empty
				              	}
				};

				PrintOptions(0, _command.CommandType, _instance, section);
			}
			else
			{
				var sectionRows = _commands.Select(c => new SectionRow
				{
				    Cells = new[] 
				                {
				                    BuildDisplayName(string.Empty, c.Attribute.Prototype),
				                    c.Attribute.Descripion
				                }
				});

				var section = new Section
				              	{
				              		Header = new[]
				              		         	{
				              		         		" - type '{command} --help' to see the help for a given command",
				              		         		"Commands:"
				              		         	},
									Rows = sectionRows
				              	};

				var indentLength = 0;

				PrintSection(indentLength, section);
			}
		}

		void PrintOptions(int indentLength, Type type, object instance, Section section)
		{
			var options = ArgsHelper.GetOptions(type).ToList();
			
			var descr = new StringBuilder();
			var rows = new List<SectionRow>(options.Count);
			foreach (var option in options.Where(o => o.ArgAttribute != null))
			{
				var defaultValue = option.PropertyInfo.GetValue(instance, null);
				bool isRequired = option.ArgAttribute.Required && defaultValue == option.PropertyInfo.PropertyType.Default();
				
				string name = BuildDisplayName("/", option.ArgAttribute.Prototype);
				if (isRequired)
				{
					name = name + " *";
				}

				descr.Length = 0;
				descr.AppendLine(option.ArgAttribute.Description);
				descr.Append("default=");
				descr.AppendLine((defaultValue ?? "{NULL}").ToString());

				if (!string.IsNullOrEmpty(option.ArgAttribute.AppSettingsKey))
				{
					descr.Append("AppSettings key=");
					descr.AppendLine(option.ArgAttribute.AppSettingsKey);
				}
				if (!string.IsNullOrEmpty(option.ArgAttribute.ConnectionStringKey))
				{
					descr.Append("ConnectionString key=");
					descr.AppendLine(option.ArgAttribute.ConnectionStringKey);
				}

				rows.Add(new SectionRow { Cells = new[] { name, descr.ToString() } });
			}

			section.Rows = rows;
			PrintSection(indentLength, section);

			foreach (var option in options.Where(p => p.ArgSetAttribute != null))
			{
				var optionSetInstance = option.PropertyInfo.PropertyType.Resolve();
				PrintOptions(indentLength, option.PropertyInfo.PropertyType, optionSetInstance, new Section());
			}
		}

		private void PrintSection(int indentLength, Section section)
		{
			Console.Out.WriteLine(string.Empty);
			var indent = new string(' ', indentLength);
			if (section.Header != null)
			{
				foreach (var header in section.Header)
				{
					_chunker.ChunckStringTo(header, Console.Out, indent);
					Console.Out.WriteLine(string.Empty);
				}
			}

			if (section.Rows.IsNullOrEmpty())
			{
				return;
			}

			indentLength += 2;
			indent = new string(' ', indentLength);
			var spacer = " : ";
			var maxCell1Length = section.Rows.Max(c => c.Cells[0].Length);
			var cell2StartIndex = indent.Length + maxCell1Length + spacer.Length;

			foreach (var row in section.Rows)
			{
				Console.Out.Write(indent);
				Console.Out.Write(row.Cells[0].PadRight(maxCell1Length));
				Console.Out.Write(spacer);

				var padding = 2 + cell2StartIndex;
				bool firstChunk = true;
				foreach (var chunk in _chunker.ChunkString(row.Cells[1], padding))
				{
					if (!firstChunk)
					{
						Console.Out.Write(new string(' ', cell2StartIndex));
					}
					Console.Out.WriteLine(chunk);
					firstChunk = false;
				}
				Console.Out.WriteLine(string.Empty);
			}
		}

		private class Section
		{
			public IEnumerable<string> Header { get; set; }
			public IEnumerable<SectionRow> Rows { get; set; }
			
		}

		private class SectionRow
		{
			public string[] Cells { get; set; } 
		}

		private static string BuildDisplayName(string prefix, string prototype)
		{
			return prefix + string.Join(", " + prefix, prototype.GetPrototypeArray());
		}
	}
}