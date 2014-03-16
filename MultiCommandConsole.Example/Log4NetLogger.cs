using System;
using Common.Logging;
using Common.Logging.Factory;
using log4net.Core;

namespace MultiCommandConsole.Example
{
    internal class Log4NetLogger : AbstractLogger
    {
        private readonly log4net.ILog _innerLog;

        public Log4NetLogger(log4net.ILog innerLog)
        {
            _innerLog = innerLog;
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            _innerLog.Logger.Log(GetType(), MapLevel(level), message, exception);
        }

        private static Level MapLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.All:
                    return Level.All;
                case LogLevel.Trace:
                    return Level.Trace;
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Warn:
                    return Level.Warn;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Fatal:
                    return Level.Fatal;
                case LogLevel.Off:
                    return Level.Off;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }

        public override bool IsTraceEnabled
        {
            get { return _innerLog.IsDebugEnabled; }
        }

        public override bool IsDebugEnabled
        {
            get { return _innerLog.IsDebugEnabled; }
        }

        public override bool IsErrorEnabled
        {
            get { return _innerLog.IsErrorEnabled; }
        }

        public override bool IsFatalEnabled
        {
            get { return _innerLog.IsFatalEnabled; }
        }

        public override bool IsInfoEnabled
        {
            get { return _innerLog.IsInfoEnabled; }
        }

        public override bool IsWarnEnabled
        {
            get { return _innerLog.IsWarnEnabled; }
        }
    }
}