using System;
using System.Collections.Generic;
using System.Threading;
using ObjectPrinter;
using Common.Logging;

namespace MultiCommandConsole.Example
{
	[ConsoleCommand("repeat", "repeats the entered phrase the specified number of times")]
	public class RepeatConsoleCommand : IConsoleCommand, ICanBeStopped, ICanBePaused
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (RepeatConsoleCommand));

		[Arg("text|t", "the text to be repeated", Required = true)]
		public string Text { get; set; }

		[Arg("times|n", "the number of times to repeat the text.  between 1 and 100", Required = true)]
		public int Times { get; set; }

        [Arg("sleep|s", "time to sleep in seconds between repeats")]
        public int Sleep { get; set; }

		public LogOptions LogOptions { get; set; }

		public IEnumerable<string> GetArgValidationErrors()
		{
			var errors = new List<string>();
			if (Times < 1)
			{
				errors.Add("times must be greater than 0");
			}
			if (Times > 100)
			{
				errors.Add("times must be less than 101");
			}
			return errors;
		}

		public string GetDetailedHelp()
		{
			return string.Empty;
		}

		public List<string> ExtraArgs { get; set; }

	    private int _stopped;
	    private int _paused;

		public void Run()
		{
			Log.Debug(this.Dump());

			for (int i = 0; i < Times; i++)
			{
                if (_stopped != 0)
                {
                    return;
                }

                while (_paused != 0)
                {
                    Thread.Sleep(1000);
                }

				Console.Out.WriteLine(Text);
                if (Sleep > 0)
                {
                    Thread.Sleep(Sleep * 1000);
                }
			}
		}

	    public void Stop()
	    {
	        Interlocked.Exchange(ref _stopped, 1);
	    }

	    public void Pause()
	    {
	        Interlocked.Exchange(ref _paused, 1);
	    }

	    public void Resume()
        {
            Interlocked.Exchange(ref _paused, 0);
	    }
	}
}