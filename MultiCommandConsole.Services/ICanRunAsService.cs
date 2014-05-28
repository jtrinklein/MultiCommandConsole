namespace MultiCommandConsole.Services
{
    /// <summary>
    /// Defines a command that can be run as a service
    /// </summary>
    public interface ICanRunAsService : ICanBeStopped
    {
        /// <summary>
        /// The default service name
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// The default display name
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The default description
        /// </summary>
        string Description { get; }
    }
}