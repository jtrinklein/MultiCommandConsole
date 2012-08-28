using System.Collections.Generic;

namespace MultiCommandConsole
{
	/// <summary>Defines Commands and ArgSets that have custom validation logic to perform before a command is run</summary>
	public interface IValidatable
	{
		IEnumerable<string> GetArgValidationErrors();
	}
}