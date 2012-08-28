namespace MultiCommandConsole
{
	/// <summary>
	/// Defines Commands and ArgSets that need to setup state before a command is run 
	/// and/or cleanup state after a command is run.
	/// </summary>
	public interface ISetupAndCleanup
	{
		void Setup();
		void Cleanup();
	}
}