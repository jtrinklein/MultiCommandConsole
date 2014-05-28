using System.Collections.Generic;

namespace MultiCommandConsole
{
	/// <summary>
	/// Defines members of a console command
	/// </summary>
	public interface IConsoleCommand : IValidatable
	{
		/// <summary>
		/// Returns help to be displayed for the command
		/// </summary>
		string GetDetailedHelp();

		/// <summary>
		/// Container for any args that aren't mapped to properties of the command
		/// </summary>
		List<string> ExtraArgs { get; set; }

		/// <summary>
		/// runs the command
		/// </summary>
		void Run();
	}
}