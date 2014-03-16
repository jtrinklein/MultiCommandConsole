using System;
using Common.Logging;

namespace MultiCommandConsole.Example
{
    internal class Log4NetFactoryAdapter : ILoggerFactoryAdapter
    {
        public ILog GetLogger(Type type)
        {
            return new Log4NetLogger(log4net.LogManager.GetLogger(type));
        }

        public ILog GetLogger(string name)
        {
            return new Log4NetLogger(log4net.LogManager.GetLogger(name));
        }
    }
}