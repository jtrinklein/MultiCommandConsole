namespace MultiCommandConsole
{
    /// <summary>
    /// Defines a class that can run a command
    /// </summary>
    public interface ICommandRunner
    {
        /// <summary>
        /// Run the command until it finishes.  
        /// When a stoplight is provided, command should stop running when the stoplight is red.
        /// </summary>
        void Run(string[] args, Stoplight stoplight = null);
        /// <summary>Stops the command if the command support stopping</summary>
        void Stop();
        /// <summary>Pauses the command if the command support pausing</summary>
        void Pause();
        /// <summary>Resumes the command if the command support resuming</summary>
        void Resume();
        /// <summary>Returns true if the command support stopping</summary>
        bool CanBeStopped { get; }
        /// <summary>Returns true if the command support pausing</summary>
        bool CanBePaused { get; }
    }
}