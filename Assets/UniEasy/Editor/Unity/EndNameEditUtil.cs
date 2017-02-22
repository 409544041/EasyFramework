using UnityEditor.ProjectWindowCallback;
using System;

namespace UniEasy.Edit
{
	internal class EndNameEditUtil : EndNameEditAction
	{
		public override void Action (int instanceID, string pathName, string resourceFile)
		{
			if (EndAction != null) {
				EndAction (instanceID, pathName, resourceFile);
			}
		}

		public event Action<int, string, string> EndAction;
	}
}
