namespace MultiCommandConsole
{
    /// <summary>
    /// Defines a command that can be paused. This is not yet supported by MultiCommandConsole.Services
    /// </summary>
    public interface ICanBePaused
    {
        /// <summary>
        /// Pauses the command
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the command
        /// </summary>
        void Resume();
    }
}