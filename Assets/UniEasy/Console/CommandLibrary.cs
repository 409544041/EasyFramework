using System.Collections.Generic;
using System.Linq;
using System;

namespace UniEasy.Console
{
	public static class CommandLibrary
	{
		private static Dictionary<string, ConsoleCommand> database = new Dictionary<string, ConsoleCommand> (StringComparer.OrdinalIgnoreCase);

		public static IEnumerable<ConsoleCommand> commands { get { return database.OrderBy (kv => kv.Key).Select (kv => kv.Value); } }

		public static void RegisterCommand (string command, ConsoleCommandCallback callback, string description = "", string usage = "")
		{
			RegisterCommand (command, description, usage, callback);
		}

		public static void RegisterCommand (string command, string description, string usage, ConsoleCommandCallback callback)
		{
			database [command] = new ConsoleCommand (command, description, usage, callback);
		}

		public static string ExecuteCommand (string command, params string[] args)
		{
			try {
				ConsoleCommand retrievedCommand = GetCommand (command);
				return retrievedCommand.callback (args);
			} catch (NoSuchCommandException e) {
				return e.Message;
			}
		}

		public static bool TryGetCommand (string command, out ConsoleCommand result)
		{
			try {
				result = GetCommand (command);
				return true;
			} catch (NoSuchCommandException) {
				result = default(ConsoleCommand);
				return false;
			}
		}

		public static ConsoleCommand GetCommand (string command)
		{
			if (HasCommand (command)) {
				return database [command];
			} else {
				command = command.ToUpper ();
				throw new NoSuchCommandException ("Command " + command + " not found.", command);
			}
		}

		public static bool HasCommand (string command)
		{
			return database.ContainsKey (command);
		}
	}
}
