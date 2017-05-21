using System.Text;

namespace UniEasy.Console
{
	public static class HelpCommand
	{
		public static readonly string name = "Help";
		public static readonly string description = "Display the list of available commands or details about a specific command.";
		public static readonly string usage = "Help [command]";

		private static StringBuilder commandList = new StringBuilder ();

		public static string Execute (params string[] args)
		{
			if (args.Length == 0) {
				return DisplayAvailableCommands ();
			} else {
				return DisplayCommandDetails (args [0]);
			}
		}

		private static string DisplayAvailableCommands ()
		{
			commandList.Length = 0;
			commandList.Append ("<b>Available Commands</b>\n");

			foreach (ConsoleCommand command in CommandLibrary.commands) {
				commandList.Append (string.Format ("    <b>{0}</b> - {1}\n", command.Name, command.Description));
			}

			commandList.Append ("To display details about a specific command, type 'HELP' followed by the command name.");
			return commandList.ToString ();
		}

		private static string DisplayCommandDetails (string commandName)
		{
			var formatting = @"<b>{0} Command</b> <b>Description:</b> {1} <b>Usage:</b> {2}";

			try {
				var command = CommandLibrary.GetCommand (commandName);
				return string.Format (formatting, command.Name, command.Description, command.Usage);
			} catch (NoSuchCommandException exception) {
				return string.Format ("Cannot find help information about {0}. Are you sure it is a valid command?", exception.command);
			}
		}
	}
}
