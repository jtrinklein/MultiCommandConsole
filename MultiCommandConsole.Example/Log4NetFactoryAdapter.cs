using System;
using System.IO;
using System.Linq;
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
            assembly = assembly ?? Assembly.GetEntryAssembly();

            var paths = new[]
                {
                    Path.GetDirectoryName(assembly.Location) + "\\log4net.config",
                    assembly.Location + ".config"
                };

            var config = paths.FirstOrDefault(File.Exists);
            if (config == null)
            {
                throw new FileNotFoundException("Unable to locate log4net configs. tried:" + string.Join(",", paths));
            }

            return new FileInfo(config);
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