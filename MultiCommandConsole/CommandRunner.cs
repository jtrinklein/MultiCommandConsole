using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.Logging;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole
{
    internal class CommandRunner : ICommandRunner
    {
        private static readonly ILog Log = LogManager.GetLogger<CommandRunner>();

        private readonly IConsoleCommandRepository _consoleCommandRepository;
        private volatile CommandRunData _runData;
        private Stoplight _stoplight;
        private EventWaitHandle _commandLoaded;
        private IConsoleWriter _writer = Config.ConsoleWriter;

        public CommandRunner(IConsoleCommandRepository consoleCommandRepository)
        {
            if (consoleCommandRepository == null) throw new ArgumentNullException("consoleCommandRepository");
            _consoleCommandRepository = consoleCommandRepository;
        }

        public void Run(string[] args)
        {
            _commandLoaded = new ManualResetEvent(false);
            _stoplight = new Stoplight();

            new Thread(() => Run(args, _stoplight)).Start();

            _commandLoaded.WaitOne();
            if (Environment.UserInteractive)
            {
                ConsoleReader.Watch(
                    _stoplight,
                    CanBeStopped, CanBePaused,
                    Stop, Pause, Resume);
            }
            else
            {
                _stoplight.Token.WaitHandle.WaitOne();
            }
        }

        public bool CanBeStopped { get { return _runData.Command is ICanBeStopped; } }

        public bool CanBePaused { get { return _runData.Command is ICanBePaused; } }

        public void Stop()
        {
            var stoppable = _runData.Command as ICanBeStopped;
            if (stoppable != null)
            {
                Log.Info("stopping");
                _writer.WriteLine("stopping");
                stoppable.Stop();
                _stoplight.Stop();
            }
            else
            {
                Log.InfoFormat("{0} does not implement {1}", _runData.Command.GetType(), typeof(ICanBeStopped));
            }
        }

        public void Pause()
        {
            var pausable = _runData.Command as ICanBePaused;
            if (pausable != null)
            {
                Log.Info("pausing");
                _writer.WriteLine("pausing");
                pausable.Pause();
            }
            else
            {
                Log.InfoFormat("{0} does not implement {1}", _runData.Command.GetType(), typeof(ICanBePaused));
            }
        }

        public void Resume()
        {
            var pausable = _runData.Command as ICanBePaused;
            if (pausable != null)
            {
                Log.Info("resuming");
                _writer.WriteLine("resuming");
                pausable.Resume();
            }
            else
            {
                Log.InfoFormat("{0} does not implement {1}", _runData.Command.GetType(), typeof(ICanBePaused));
            }
        }

        public void Run(string[] args, Stoplight stoplight)
        {
            DateTime started = Config.NowDelegate();
            var stopwatch = Stopwatch.StartNew();

            Log.InfoFormat("Running: {0}", args.IsNullOrEmpty() ? "{null}" : string.Join(" ", args));
            if (Config.ConsoleMode.ArgumentsInterceptor != null)
            {
                var modifiedArgs = Config.ConsoleMode.ArgumentsInterceptor(args);
                if (!args.SequenceEqual(modifiedArgs))
                {
                    args = modifiedArgs;
                    Log.InfoFormat("Running modified args: {0}", string.Join(" ", args));
                }
            }

            if (Config.ConsoleMode.OnBeginRunCommand != null)
            {
                Config.ConsoleMode.OnBeginRunCommand();
            }

            //load command after OnBeginRunCommand to let DI containers be configured
            _runData = _consoleCommandRepository.LoadCommand(args);
            if (_runData.Errors.Any())
            {
                var writer = new ConsoleWriter();
                foreach (var error in _runData.Errors)
                {
                    writer.WriteLines("", "!!!", error, "");
                }
            }

            _runData.Command.SetServiceOnPropertyOrField((IStoplight)stoplight);

            _commandLoaded.Set();

            //TODO: exclude console command
            if (CanBeStopped)
            {
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                Console.CancelKeyPress += OnCancelKeyPress;
            }

            RunSafely(_runData);

            if (CanBeStopped)
            {
                Console.CancelKeyPress -= OnCancelKeyPress;
                AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            }

            if (Config.ConsoleMode.OnEndRunCommand != null)
            {
                Config.ConsoleMode.OnEndRunCommand();
            }

            Log.InfoFormat("Command execution took:{0} started:{1} ended:{2}",
                           stopwatch.Elapsed,
                           started.ToString("hh:mm:ss"),
                           Config.NowDelegate().ToString("hh:mm:ss"));

            stoplight.Stop();
        }

        private void RunSafely(CommandRunData runData)
        {
            try
            {
                runData.SetterUppers.ForEach(su => su.Setup());

                try
                {
                    runData.Command.Run();
                }
                finally
                {
                    runData.SetterUppers.Reverse();
                    runData.SetterUppers.ForEach(su =>
                        {
                            try
                            {
                                su.Cleanup();
                            }
                            catch (Exception e)
                            {
                                e.SetContext("cleaner upper", su);
                                Log.ErrorFormat("failed cleanup for {0}", e, su.GetType().Name);
                            }
                        });
                }
            }
            catch (TargetInvocationException e)
            {
                if (runData != null && runData.Command != null)
                {
                    e.SetContext("command", runData.Command);
                }
                _writer.WriteLine(e.Message + " see logs for details");
                var error = (e.InnerException ?? e).DumpToString();
                Log.Error(error);
            }
            catch (Exception e)
            {
                if (runData != null && runData.Command != null)
                {
                    e.SetContext("command", runData.Command);
                }
                _writer.WriteLine(e.Message + " see logs for details");
                var error = e.DumpToString();
                Log.Error(error);
            }
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