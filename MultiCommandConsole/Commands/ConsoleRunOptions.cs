using System;
using Common.Logging;
using MultiCommandConsole.Util;

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
        /// If the command implements ICanBeCancelled
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Console.CancelKeyPress += OnCancelKeyPress;

            _runner = CreateCommandRunner();
            try
            {
                var stoplight = _runner.Run(args);
                ConsoleReader.Watch(Stop, Pause, Resume, until: () => stoplight.IsRed);
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
            if (!_runner.CanBeCancelled)
            {
                Console.Out.WriteLine("the command cannot be cancelled");
                return;
            }

            Console.Out.WriteLine("stopping");
            _runner.Stop();
        }

        private void Pause()
        {
            if (!_runner.CanBePaused)
            {
                Console.Out.WriteLine("the command cannot be paused");
                return;
            }

            Console.Out.WriteLine("pausing");
            _runner.Pause();
        }

        private void Resume()
        {
            if (!_runner.CanBePaused)
            {
                Console.Out.WriteLine("the command cannot be paused, therefore it cannot be resumed");
                return;
            }

            Console.Out.WriteLine("resuming");
            _runner.Resume();
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
            Log.Info("ConsoleRunOptions.OnCancelKeyPress");
            e.Cancel = true;
            Stop();
        }
    }
}