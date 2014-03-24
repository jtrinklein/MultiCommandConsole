using System;
using System.Threading;

namespace MultiCommandConsole.Util
{
    internal class TemporaryThread : IDisposable
    {
        private readonly Thread _inputThread;
        private readonly EventWaitHandle _actionCompleted;

        public TemporaryThread(Action action)
        {
            _actionCompleted = new ManualResetEvent(false);
            _inputThread = new Thread(() => Start(action)) { IsBackground = true };
        }

        public void RunOnce()
        {
            _inputThread.Start();
            _actionCompleted.WaitOne();
        }

        public bool RunUntil(Func<bool> until, int checkEveryNMilliSeconds = 100)
        {
            _inputThread.Start();

            while (!until())
            {
                var success =_actionCompleted.WaitOne(checkEveryNMilliSeconds);
                return true;
            }
            return false;
        }

        private void Start(Action action)
        {
            action();
            _actionCompleted.Set();
        }

        public void Dispose()
        {
            _actionCompleted.Dispose();
        }
    }
}