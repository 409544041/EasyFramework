using System;

namespace UniEasy.Console
{
	public sealed class CommandDisposable : IDisposable
	{
		public string Name { get; private set; }

		internal CommandDisposable (string name)
		{
			Name = name;
		}

		public void Dispose ()
		{
			CommandLibrary.DeregisterCommand (Name);
		}
	}
}
