using System;
using System.Text;
using System.Threading;

namespace MultiCommandConsole.Util
{
    public static class ConsoleReader
    {
        public static void Watch(IStoplight stoplight, bool canBeStopped, bool canBePaused, Action onStop, Action onPause, Action onResume, int checkEveryNMilliSeconds = 100)
        {
            //all this because Ctrl+C is broken in .net 4.0

            if (!Environment.UserInteractive)
            {
                throw new InvalidOperationException("expected UserInteractive mode");
            }

            ShowConsoleOptions(canBeStopped, canBePaused);
            bool isPaused = false;

            while (stoplight.IsGreen)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (isPaused)
                    {
                        if (key.Key == ConsoleKey.R)
                        {
                            onResume();
                            isPaused = false;
                            ShowConsoleOptions(canBeStopped, canBePaused);
                        }
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.S)
                        {
                            onStop();
                        }
                        else if (key.Key == ConsoleKey.P)
                        {
                            isPaused = true;
                            onPause();
                            ShowConsoleOptions(canBeStopped, canBePaused, isPaused: true);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(checkEveryNMilliSeconds);
                }
            }
        }

        public static void ShowConsoleOptions(bool canBeStopped, bool canBePaused, bool isPaused = false)
        {
            var sb = new StringBuilder();
            if (canBeStopped)
            {
                sb.Append("(s)top ");
            }
            if (canBePaused)
            {
                sb.Append(isPaused ? "(r)esume" : "(p)ause");
            }
            if (sb.Length == 0)
            {
                return;
            }
            Console.WriteLine();
            Console.Write("options: ");
            Console.WriteLine(sb);
            Console.WriteLine();
        }
    }
}