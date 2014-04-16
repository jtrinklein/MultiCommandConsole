using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.Logging;
using MultiCommandConsole.Commands;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole
{
    public class CommandRunner : ICommandRunner
    {
        private static readonly ILog Log = LogManager.GetLogger<CommandRunner>();
        private static readonly IConsoleWriter Writer = ConsoleWriter.Get<CommandRunner>();

        private readonly IConsoleCommandRepository _consoleCommandRepository;
        private volatile CommandRunData _runData;
        private Stoplight _stoplight;
        private EventWaitHandle _commandLoaded;

        public CommandRunner(IConsoleCommandRepository consoleCommandRepository)
        {
            if (consoleCommandRepository == null) throw new ArgumentNullException("consoleCommandRepository");
            _consoleCommandRepository = consoleCommandRepository;
        }

        public void Run(string[] args, Stoplight stoplight = null)
        {
            _commandLoaded = new ManualResetEvent(false);
            _stoplight = stoplight ?? new Stoplight();

            new Thread(() => RunOnThread(args, _stoplight)).Start();

            _commandLoaded.WaitOne();
            if (Environment.UserInteractive)
            {
                ConsoleReader.Watch(
                    _stoplight,
                    CanBeStopped, CanBePaused,
                    Stop, Pause, Resume);
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
                Writer.WriteLine("stopping");
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
                Writer.WriteLine("pausing");
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
                Writer.WriteLine("resuming");
                pausable.Resume();
            }
            else
            {
                Log.InfoFormat("{0} does not implement {1}", _runData.Command.GetType(), typeof(ICanBePaused));
            }
        }

        private void RunOnThread(string[] args, Stoplight stoplight)
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

            IDisposable cleanup = null;
            if (Config.ConsoleMode.OnWrapRunCommand != null)
            {
                Log.Info("call OnWrapRunCommand");
                cleanup = Config.ConsoleMode.OnWrapRunCommand();
            }

            try
            {
//load command after OnWrapRunCommand to let DI containers be configured
                _runData = _consoleCommandRepository.LoadCommand(args);

                if (Config.ConsoleMode.OnBeginRunCommand != null)
                {
                    Log.Info("call OnBeginRunCommand");
                    Config.ConsoleMode.OnBeginRunCommand(_runData.Command);
                }

                if (_runData.Errors.Any())
                {
                    foreach (var error in _runData.Errors)
                    {
                        Writer.WriteLines("", "!!!", error, "");
                    }
                }

                _runData.Command.SetServiceOnPropertyOrField((IStoplight)stoplight);

                _commandLoaded.Set();
                Log.Info("command loaded");

                //TODO: exclude console command
                if (CanBeStopped)
                {
                    Log.Info("CanBeStopped: subscribe unhandled exceptions and cancel key press");
                    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                    Console.CancelKeyPress += OnCancelKeyPress;
                }

                RunSafely(_runData);

                if (CanBeStopped)
                {
                    Log.Info("CanBeStopped: unsubscribe unhandled exceptions and cancel key press");
                    Console.CancelKeyPress -= OnCancelKeyPress;
                    AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
                }
            }
            finally
            {
                if (cleanup != null)
                {
                    Log.Info("dispose OnWrapRunCommand");
                    cleanup.Dispose();
                } 
            }

            if (Config.ConsoleMode.OnEndRunCommand != null)
            {
                Log.Info("call OnEndRunCommand");
                Config.ConsoleMode.OnEndRunCommand(new RunTime
                    {
                        Elapsed = stopwatch.Elapsed,
                        StartedOn = started,
                        EndedOn = Config.NowDelegate()
                    });
            }

            var runTimeMsg = string.Format("Command execution took:{0} started:{1} ended:{2}",
                                           stopwatch.Elapsed,
                                           started.ToString("hh:mm:ss"),
                                           Config.NowDelegate().ToString("hh:mm:ss"));

            if (!(_runData.Command is HelpCommand) 
                && !(_runData.Command is ViewArgsCommand))
            {
                if (Config.WriteRunTimeToConsole)
                {
                    Writer.WriteLine(runTimeMsg);
                }
                else
                {
                    Log.InfoFormat(runTimeMsg);
                }
            }

            stoplight.Stop();
        }

        private void RunSafely(CommandRunData runData)
        {
            try
            {
                runData.SetterUppers.ForEach(su =>
                    {
                        Log.InfoFormat("{0}.Setup()", su.GetType().Name);
                        su.Setup();
                    });

                try
                {
                    Log.InfoFormat("{0}.Run()", runData.Command.GetType().Name);
                    runData.Command.Run();
                }
                finally
                {
                    runData.SetterUppers.Reverse();
                    runData.SetterUppers.ForEach(su =>
                        {
                            try
                            {
                                Log.InfoFormat("{0}.Cleanup()", su.GetType().Name);
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
                Writer.WriteLine(e.Message + " see logs for details");
                var error = (e.InnerException ?? e).DumpToString();
                Log.Error(error);
            }
            catch (Exception e)
            {
                if (runData != null && runData.Command != null)
                {
                    e.SetContext("command", runData.Command);
                }
                Writer.WriteLine(e.Message + " see logs for details");
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

    public class RunTime
    {
        public DateTime StartedOn { get; set; }
        public DateTime EndedOn { get; set; }
        public TimeSpan Elapsed { get; set; }
    }
}