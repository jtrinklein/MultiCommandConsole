using System;

namespace MultiCommandConsole.Util
{
    public class ConsoleLogger : ILogger
    {
        public void RunCurrentCommand(IConsoleCommand command)
        {
            Console.Out.WriteLine("run: {0}", command);
        }

        public void Debug(string message)
        {
            Console.Out.WriteLine("debug: {0}", message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Console.Out.WriteLine("debug: {0}", string.Format(format, args));
        }

        public void InfoFormat(string format, params object[] args)
        {
            Console.Out.WriteLine("info: {0}", string.Format(format, args));
        }

        public void Error(string message)
        {
            Console.Out.WriteLine("error: {0}", message);
        }

        public void ErrorFormat(Exception ex, string format, params object[] args)
        {
            Console.Out.WriteLine("error: {0}", string.Format(format, args));
        }
    }
}