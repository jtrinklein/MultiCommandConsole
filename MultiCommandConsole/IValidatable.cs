using System.Collections.Generic;

namespace MultiCommandConsole
{
	public interface IValidatable
	{
		IEnumerable<string> GetArgValidationErrors();
	}
}