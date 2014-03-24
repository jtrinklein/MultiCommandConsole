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
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Console.CancelKeyPress += OnCancelKeyPress;

            _runner = CreateCommandRunner();
            try
            {
                _runner.Run(args);
            }
            finally
            {
                _runner = null;
                Console.CancelKeyPress -= OnCancelKeyPress;
                AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            }
        }

        private void Stop()
        {
            if (!_runner.CanBeStopped)
            {
                Console.Out.WriteLine("the command cannot be cancelled");
                return;
            }

            _runner.Stop();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            if (eventArgs.IsTerminating)
            {
                Log.Fatal("unhandled exception. stopping service.", (Exception)eventArgs.ExceptionObject);
                Stop();
            }
            else
            {
                Log.Error("unhandled exception", (Exception)eventArgs.ExceptionObject);
            }
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Stop();
        }
    }
}