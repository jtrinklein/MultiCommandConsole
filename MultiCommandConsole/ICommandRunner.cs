namespace MultiCommandConsole
{
    public interface ICommandRunner
    {
        void Run(string[] args);
        void Stop();
        void Pause();
        void Resume();
        bool CanBeStopped { get; }
        bool CanBePaused { get; }
    }
}