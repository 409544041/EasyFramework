using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System;

namespace UniEasy.Edit
{
	internal class EndNameEdit : EndNameEditAction
	{
		#region implemented abstract members of EndNameEditAction

		public override void Action (int instanceID, string pathName, string resourceFile)
		{
			if (EndAction != null) {
				EndAction (instanceID, pathName, resourceFile);
			}
		}

		public event Action<int, string, string> EndAction;

		#endregion
	}

	public class StartNameEditor : EditorWindow
	{
		static public void Create (int InstanceID, EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
		{
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists (
				InstanceID,
				endAction,
				pathName,
				icon, 
				resourceFile);
		}
	}
}
