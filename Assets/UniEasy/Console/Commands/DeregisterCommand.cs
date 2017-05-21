using System.Collections.Generic;
using System.Linq;
using System;

namespace UniEasy.Console
{
	public static class DeregisterCommand
	{
		public static readonly string name = "Deregister";
		public static readonly string description = "Deregister a specific command.";
		public static readonly string usage = "Deregister commands";

		public static string Execute (params string[] args)
		{
			var result = "";
			var failed = new List<string> ();
			for (int i = 0; i < args.Length; i++) {
				if (CommandLibrary.DeregisterCommand (args [i])) {
					if (string.IsNullOrEmpty (result)) {
						result += string.Format ("Success to deregister command [{0}]", args [i]);
					} else {
						result += Environment.NewLine + string.Format ("Success to deregister command [{0}]", args [i]);
					}
				} else {
					failed.Add (string.Format ("Failed to deregister command [{0}]", args [i]));
				}
			}
			for (int i = 0; i < failed.Count; i++) {
				if (string.IsNullOrEmpty (result)) {
					result += failed [i];
				} else {
					result += Environment.NewLine + failed [i];
				}
			}
			return result;
		}
	}
}
