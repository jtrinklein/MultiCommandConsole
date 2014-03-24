using System.Threading;

namespace MultiCommandConsole
{
    /// <summary>
    /// A stoplight is a multi-threaded pattern letting a thread know when in can continue.
    /// A stoplight is a wrapper around a CancellationTokenSource.
    /// </summary>
    public interface IStoplight
    {
        CancellationToken Token { get; }
        bool IsGreen { get; }
        bool IsRed { get; }
    }
}