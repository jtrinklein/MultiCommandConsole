namespace MultiCommandConsole
{
    public interface ICommandRunner
    {
        void Run(string[] args, Stoplight stoplight = null);
        void Stop();
        void Pause();
        void Resume();
        bool CanBeStopped { get; }
        bool CanBePaused { get; }
    }
}