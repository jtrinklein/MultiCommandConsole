using System;
using System.IO;
using System.Reflection;
using Common.Logging;

namespace MultiCommandConsole.Example
{
    /// <summary>
    /// Uses config file to configure Log4Net.  
    /// Reads from local log4net.config file first, 
    /// then any specified assembly config, 
    /// then the entry assembly config.
    /// </summary>
    public class Log4NetFactoryAdapter : ILoggerFactoryAdapter
    {
        public static Log4NetFactoryAdapter Load()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(GetLogFileInfo());
            return new Log4NetFactoryAdapter();
		}

        private static FileInfo GetLogFileInfo(Assembly assembly = null)
        {
            if (File.Exists("log4net.config"))
            {
                return new FileInfo("log4net.config");
            }

            var appConfig = (assembly ?? Assembly.GetEntryAssembly()).Location + ".config";
            if (File.Exists(appConfig))
            {
                return new FileInfo(appConfig);
            }
            throw new FileNotFoundException("Unable to locate log4net configs at log4net.config or " + appConfig);
        }

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