using System.ServiceProcess;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Services
{
    public class ServiceCommandRunner : ServiceBase, ICommandRunner
    {
        private readonly ICommandRunner _innerRunner;

        public ServiceCommandRunner(ICommandRunner innerRunner)
        {
            _innerRunner = innerRunner;
        }

        protected override void OnStart(string[] args)
        {
            //args will be empty when running as a service

            var currentService = new ServicesRepository().GetCurrent();
            _innerRunner.Run(currentService.CommandLine.SplitCmdLineArgs());
            CanStop = _innerRunner.CanBeStopped;
            CanPauseAndContinue = _innerRunner.CanBePaused;
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

        void ICommandRunner.Run(string[] args)
        {
            Run(this);
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