using System;

namespace UniEasy.Console
{
	public delegate string ConsoleCommandCallback (params string[] args);

	public struct ConsoleCommand
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Usage { get; set; }

		public ConsoleCommandCallback Callback { get; set; }

		public CommandDisposable Disposer { get; set; }

		public ConsoleCommand (string name, string description, string usage, ConsoleCommandCallback callback) : this ()
		{
			Name = name;
			Description = (string.IsNullOrEmpty (description.Trim ()) ? "No description provided" : description);
			Usage = (string.IsNullOrEmpty (usage.Trim ()) ? "No usage information provided" : usage);
			Callback = callback;
			Disposer = new CommandDisposable (Name);
		}
	}
}
