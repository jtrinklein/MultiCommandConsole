using System;

namespace MultiCommandConsole.Util
{
	internal class DisposableAction : IDisposable
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