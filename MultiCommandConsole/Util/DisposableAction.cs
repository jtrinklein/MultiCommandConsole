using System;

namespace MultiCommandConsole.Util
{
	public class DisposableAction : IDisposable
	{
		readonly Action _action;

		public DisposableAction(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}
	}
}