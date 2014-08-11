using System;
using System.IO;
using System.ServiceProcess;
using Common.Logging;
using MultiCommandConsole.Util;
using ObjectPrinter;

namespace MultiCommandConsole.Services
{
    public class ServiceCommandRunner : ServiceBase, ICommandRunner
    {
        private static readonly ILog Log = LogManager.GetLogger<ServiceCommandRunner>();

        private readonly ICommandRunner _innerRunner;
        private Stoplight _stoplight;

        public ServiceCommandRunner(ICommandRunner innerRunner)
        {
            _innerRunner = innerRunner;
        }

        protected override void OnStart(string[] args)
        {
            //args will be empty when running as a service

            Service currentService = null;

            try
            {
                currentService = new ServicesRepository().GetCurrent();
                _innerRunner.Run(currentService.CommandLine.SplitCmdLineArgs(), _stoplight);
                Log.Debug("OnStart completed");
            }
            catch (Exception e)
            {
                e.SetContext("args", args);
                e.SetContext("currentService", currentService);
                Log.Error(e);
                throw;
            }
        }

        protected override void OnStop()
        {
            _innerRunner.Stop();
        }

        protected override void OnPause()
        {
            _innerRunner.Pause();
        }

        protected override void OnContinue()
        {
            _innerRunner.Resume();
        }

        void ICommandRunner.Run(string[] args, Stoplight stoplight)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            _stoplight = stoplight ?? new Stoplight();

            //TODO: figure out how to load the service config before the service is running.
            //      CanStop and CanPauseAndContinue can only be set before the service starts
            //      but we can't get the current service until there's a process id.  ARG!!!!
            CanStop = true;

            Run(this);

            Log.Debug("Run(this) completed");
        }

        void ICommandRunner.Stop()
        {
            Stop();
        }

        void ICommandRunner.Pause()
        {
            _innerRunner.Pause();
        }

        void ICommandRunner.Resume()
        {
            _innerRunner.Resume();
        }

        bool ICommandRunner.CanBeStopped { get { return _innerRunner.CanBeStopped; } }
        bool ICommandRunner.CanBePaused { get { return _innerRunner.CanBePaused; } }
    }
}