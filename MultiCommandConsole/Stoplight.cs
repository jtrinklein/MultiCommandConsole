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

        /// <summary></summary>
        public Stoplight(CancellationTokenSource cancellationTokenSource = null)
        {
            _source = cancellationTokenSource ?? new CancellationTokenSource();
            _wait = new ManualResetEventSlim();
        }

        /// <summary></summary>
        public CancellationToken Token
        {
            get { return _source.Token; }
        }

        /// <summary></summary>
        public bool IsGreen
        {
            get { return !IsRed; }
        }

        /// <summary></summary>
        public bool IsRed
        {
            get { return _source.IsCancellationRequested; }
        }

        /// <summary>
        /// calls cancel on underlying CancellationTokenSource
        /// </summary>
        public void Stop()
        {
            if (_source.IsCancellationRequested)
            {
                return;
            }
            Log.Info("signaling service to stop");
            _source.Cancel();
        }

        /// <summary></summary>
        public void Sleep(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero || IsRed)
            {
                return;
            }
            try
            {
                _wait.Wait(timeout, Token);
            }
            catch (OperationCanceledException)
            {
                //expected.  nothing to do here except return
            }
        }
    }
}