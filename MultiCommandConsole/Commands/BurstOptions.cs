using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MultiCommandConsole.Util;

namespace MultiCommandConsole.Commands
{
    [ArgSet("repeat", "repeat operation until completed")]
    public class BurstOptions : IValidatable
    {
        private static readonly IConsoleWriter Writer = ConsoleWriter.Get<BurstOptions>();

        private readonly IStoplight _stoplight;
        private volatile bool _run;
        private int _count;
        private EventWaitHandle _wait;
        private IList<BurstSegment> _burstSegments;

        public long TotalCount
        {
            get
            {
                return _burstSegments == null
                           ? 0
                           : RepeatPattern < 0
                                 ? -1
                                 : _burstSegments.Sum(s => (long)s.Send * s.Repeat) * (RepeatPattern + 1);
            }
        }

        [Arg("burstPattern|bp", "allows specifying a pattern to run the command in bursts. " +
                                "\ns=send  w=wait  r=repeat " +
                                "\ns10w5r3s30w4r7 = send 10, wait 5sec, repeat 3x, send 30, wait 4sec, repeat 7x." +
                                "\nuse -1 to repeat infinitely i.e. s10w5r-1")]
        public string BurstPattern { get; set; }

        [Arg("repeat", "repeats burstPattern the specified number of times.  0=don't repeate  -1=repeat infinitely")]
        public int RepeatPattern { get; set; }

        public BurstOptions(IStoplight stoplight)
        {
            _stoplight = stoplight;
            var segment = new BurstSegment { Send = 1 };
            _burstSegments = new[] { segment };
            BurstPattern = segment.ToString();
        }

        public void Run(Action<BurstSegment> actOnBurst = null, Action<int> actOnIndex = null)
        {
            _run = true;
            var loops = 0;

            do
            {
                foreach (var burstSegment in _burstSegments)
                {
                    Writer.WriteLine("begin burst segment: {0}", burstSegment);
                    int repeats = 0;
                    while (burstSegment.Repeat < 0 || burstSegment.Repeat > repeats)
                    {
                        if (actOnIndex != null)
                        {
                            foreach (var index in Enumerable.Range(_count + 1, burstSegment.Send))
                            {
                                actOnIndex(index);
                            }
                            _count = burstSegment.Send;
                        }
                        if (actOnBurst != null)
                        {
                            actOnBurst(burstSegment);
                        }

                        if (burstSegment.Wait > 0)
                        {
                            _stoplight.Sleep(TimeSpan.FromSeconds(burstSegment.Wait));
                        }

                        if (_wait != null)
                        {
                            _wait.WaitOne();
                        }
                        if (!_run)
                        {
                            return;
                        }

                        repeats++;
                    }
                }
                loops++;
            } while (RepeatPattern == -1 || loops <= RepeatPattern);
        }

        public void Stop()
        {
            _run = false;
            if (_wait != null)
            {
                _wait.Set();
                _wait = null;
            }
        }

        public void Pause()
        {
            _wait = new ManualResetEvent(false);
        }

        public void Resume()
        {
            _wait.Set();
            _wait = null;
        }

        public IEnumerable<string> GetArgValidationErrors()
        {
            if (string.IsNullOrWhiteSpace(BurstPattern))
            {
                return new[] { "invalid burstPattern: " + BurstPattern };
            }
            _burstSegments = BurstSegment.ParseSegments(BurstPattern);
            return Enumerable.Empty<string>();
        }
    }
}