using System;
using Common.Logging;

namespace MultiCommandConsole.Commands
{
    [ArgSet("console", "options for long running console commands")]
    public class ConsoleRunOptions
    {
        private static readonly ILog Log = LogManager.GetLogger<ConsoleRunOptions>();

        private volatile ICommandRunner _runner;

        internal Func<ICommandRunner> CreateCommandRunner { private get; set; }

        /// <summary>
        /// Synchronous run method that will run the command for the given args in a new thread.
        /// If the command implements ICanBeStopped
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {

            _runner = CreateCommandRunner();
            try
            {
                _runner.Run(args);
            }
            finally
            {
                _runner = null;
            }
        }
    }
}