using System;
using System.Threading;

namespace MultiCommandConsole
{
    /// <summary>
    /// A stoplight is a multi-threaded pattern letting a thread know when in can continue.
    /// A stoplight is a wrapper around a CancellationTokenSource.
    /// </summary>
    public interface IStoplight
    {
        /// <summary>
        /// The CancellationToken
        /// </summary>
        CancellationToken Token { get; }

        /// <summary>
        /// When true, cancel has not been called
        /// </summary>
        bool IsGreen { get; }

        /// <summary>
        /// When true, cancel has been called
        /// </summary>
        bool IsRed { get; }

        /// <summary>
        /// Same as Thread.Sleep, but thread wake when cancel is called.
        /// Prevents app from not closing while waiting for Thread.Sleep to finish.
        /// </summary>
        /// <param name="timeout"></param>
        void Sleep(TimeSpan timeout);
    }
}