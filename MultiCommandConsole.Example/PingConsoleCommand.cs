using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Common.Logging;
using MultiCommandConsole.Services;
using ObjectPrinter;
using Timer = System.Timers.Timer;

namespace MultiCommandConsole.Example
{
    [ConsoleCommand("ping", "pings the specified sites every N minutes and reports success and failure")]
    public class PingConsoleCommand : IConsoleCommand, ICanRunAsService, ICanBePaused
    {
        private static readonly ILog Log = LogManager.GetLogger<PingConsoleCommand>();

        public string ServiceName { get { return "SimplePingService"; } }
        public string DisplayName { get { return "Simple Ping Service"; } }
        public string Description
        {
            get
            {
                return
                    string.Format("pings the following site(s) every {0} minutes and reports success and failure: {1}",
                                  Interval,
                                  string.Join(",", Sites));
            }
        }

        [Arg("sites|s", "comma delimited list of sites")]
        public string[] Sites { get; set; }

        [Arg("interval|i", "interval in minutes")]
        public int Interval { get; set; }

        [Arg("timeout|to", "time to wait for ping to succeed per site")]
        public int Timeout { get; set; }

        public IEnumerable<string> GetArgValidationErrors()
        {
            var errors = new List<string>();
            if (Sites == null || Sites.Length == 0)
            {
                errors.Add("at least one site must be specified");
            }
            if (Interval < 1)
            {
                errors.Add("interval must be greater than 0");
            }
            return errors;
        }

        public string GetDetailedHelp()
        {
            return "";
        }

        public List<string> ExtraArgs { get; set; }

        private volatile bool _shouldStop;
        private volatile bool _shouldPause;
        private readonly EventWaitHandle _resume = new AutoResetEvent(false);
        private Timer _timer;

        public void Run()
        {
            _timer = new Timer(Interval);
            _timer.Elapsed += (sender, args) => PingSites();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _shouldStop = true;
        }

        public void Pause()
        {
            _timer.Stop();
            _shouldPause = true;
        }

        public void Resume()
        {
            _shouldPause = false;
            _resume.Set();
            _timer.Start();
        }

        private void PingSites()
        {
            foreach (var site in Sites)
            {
                try
                {
                    var reply = Timeout > 0
                                    ? new Ping().Send(site, Timeout)
                                    : new Ping().Send(site);

                    if (reply != null && reply.Status == IPStatus.Success)
                    {
                        Log.InfoFormat("successful ping: {0} ({1}) took {2}", site, reply.Address,
                                       TimeSpan.FromMilliseconds(reply.RoundtripTime));
                    }
                    else
                    {
                        Log.ErrorFormat("failed ping: {0} \n {1}", site, reply.DumpToString());
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("failed ping: {0} \n {1}", site, e.DumpToString());
                }

                if (_shouldStop)
                {
                    return;
                }
                if (_shouldPause)
                {
                    _resume.WaitOne();
                }
            }
        }
    }
}