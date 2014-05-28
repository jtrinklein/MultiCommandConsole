using System.Collections.Generic;

namespace MultiCommandConsole
{
	/// <summary>Defines Commands and ArgSets that have custom validation logic to perform before a command is run</summary>
	public interface IValidatable
	{
		/// <summary>
		/// validates provided args and returns any errors
		/// </summary>
		IEnumerable<string> GetArgValidationErrors();
	}
}