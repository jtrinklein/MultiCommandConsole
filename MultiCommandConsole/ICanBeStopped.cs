namespace MultiCommandConsole
{
    /// <summary>
    /// Defines a command that can be stopped
    /// </summary>
    public interface ICanBeStopped
    {
        /// <summary>
        /// Stops the command
        /// </summary>
        void Stop();
    }
}