using System.Runtime.Serialization;
using System;

namespace UniEasy.Console
{
	[Serializable]
	public class NoSuchCommandException : Exception
	{
		public string command { get; private set; }

		public NoSuchCommandException () : base ()
		{
		}

		public NoSuchCommandException (string message) : base (message)
		{
		}

		public NoSuchCommandException (string message, string command)
			: base (message)
		{
			this.command = command;
		}

		protected NoSuchCommandException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
			if (info != null)
				this.command = info.GetString ("command");
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			if (info != null)
				info.AddValue ("command", command);
		}
	}
}
