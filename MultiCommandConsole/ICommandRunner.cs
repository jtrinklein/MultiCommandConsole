namespace MultiCommandConsole
{
    public interface ICommandRunner
    {
        void Run(string[] args);
        void Stop();
    }
}