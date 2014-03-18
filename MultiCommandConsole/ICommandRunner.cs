using System.Threading;

namespace MultiCommandConsole
{
    public interface ICommandRunner
    {
        ManualResetEvent Run(string[] args);
        void Stop();
        void Pause();
        void Resume();
    }
}