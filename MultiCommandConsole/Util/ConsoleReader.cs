using System;
using System.Threading;

namespace MultiCommandConsole.Util
{
    public static class ConsoleReader
    {
        public static void Watch(Action onStop, Action onPause, Action onResume, Func<bool> until, int checkEveryNMilliSeconds = 100)
        {
            //all this because Ctrl+C is broken in .net 4.0

            ShowConsoleOptions();
            bool isPaused = false;

            while (!until())
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
                            ShowConsoleOptions();
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
                            ShowConsoleOptions(isPaused: true);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(checkEveryNMilliSeconds);
                }
            }
        }

        public static void ShowConsoleOptions(bool isPaused = false)
        {
            Console.WriteLine();
            Console.WriteLine(isPaused 
                ? "options: (s)top (r)esume" 
                : "options: (s)top (p)ause");
            Console.WriteLine();
        }
    }
}