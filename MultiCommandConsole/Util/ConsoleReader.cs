using System;
using System.Threading;
using Common.Logging;

namespace MultiCommandConsole.Util
{
    public static class ConsoleReader
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConsoleReader));

        public static bool WatchForCancel(Func<bool> until, int checkEveryNMilliSeconds = 100)
        {
            //all this because Ctrl+C is broken in .net 4.0

            while (!until())
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(false);
                    if (key.Key == ConsoleKey.Q && key.Modifiers == ConsoleModifiers.Control)
                    {
                        Log.Info("received ctrl+q.  signalling cancel");
                        return true;
                    }
                    Console.Out.Write(key);
                }
                else
                {
                    Thread.Sleep(checkEveryNMilliSeconds);
                }
            }

            return false;
        }
    }
}