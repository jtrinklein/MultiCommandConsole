using System;
using System.Threading;
using Common.Logging;

namespace MultiCommandConsole
{
    /// <summary>
    /// A stoplight is a multi-threaded pattern letting a thread know when in can continue.
    /// A stoplight is a wrapper around a CancellationTokenSource.
    /// </summary>
    public class Stoplight : IStoplight
    {
        private static readonly ILog Log = LogManager.GetLogger<Stoplight>();

        private readonly CancellationTokenSource _source;
        private readonly ManualResetEventSlim _wait;

        public Stoplight(CancellationTokenSource cancellationTokenSource = null)
        {
            _source = cancellationTokenSource ?? new CancellationTokenSource();
            _wait = new ManualResetEventSlim();
        }

        public CancellationToken Token
        {
            get { return _source.Token; }
        }

        public bool IsGreen
        {
            get { return !IsRed; }
        }

        public bool IsRed
        {
            get { return _source.IsCancellationRequested; }
        }

        public void Stop()
        {
            if (_source.IsCancellationRequested)
            {
                return;
            }
            Log.Info("signaling service to stop");
            _source.Cancel();
        }

        public void Sleep(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero || IsRed)
            {
                return;
            }
            _wait.Wait(timeout, Token);
        }
    }
}