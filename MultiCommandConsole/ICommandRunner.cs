namespace MultiCommandConsole
{
    public interface ICommandRunner
    {
        Stoplight Run(string[] args);
        void Stop();
        void Pause();
        void Resume();
        bool CanBeCancelled { get; }
        bool CanBePaused { get; }
    }
}