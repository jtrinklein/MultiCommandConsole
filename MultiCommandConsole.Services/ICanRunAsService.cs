namespace MultiCommandConsole.Services
{
    public interface ICanRunAsService : ICanBeStopped
    {
        string ServiceName { get; }
        string DisplayName { get; }
        string Description { get; }
    }
}