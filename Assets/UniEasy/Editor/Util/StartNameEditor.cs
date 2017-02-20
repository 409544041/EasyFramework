using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace UniEasy.Edit
{
	internal class EndNameEdit : EndNameEditAction
	{
		#region implemented abstract members of EndNameEditAction

		public override void Action (int instanceId, string pathName, string resourceFile)
		{
			AssetDatabase.CreateAsset (EditorUtility.InstanceIDToObject (instanceId), AssetDatabase.GenerateUniqueAssetPath (pathName));
		}

		#endregion
	}

	public class StartNameEditor : EditorWindow
	{
		static public void Rename (GameObject go)
		{
			var InstanceID = go.GetInstanceID ();
			var endAction = ScriptableObject.CreateInstance<EndNameEditAction> ();
			var pathName = AssetDatabase.GetAssetPath (InstanceID);
			var icon = AssetPreview.GetMiniThumbnail (go);
			Rename (InstanceID, endAction, pathName, icon, "");
		}

		static public void Rename (GameObject go, string resourceFile)
		{
			var InstanceID = go.GetInstanceID ();
			var endAction = ScriptableObject.CreateInstance<EndNameEditAction> ();
			var pathName = AssetDatabase.GetAssetPath (InstanceID);
			var icon = AssetPreview.GetMiniThumbnail (go);
			Rename (InstanceID, endAction, pathName, icon, resourceFile);
		}

		static public void Rename (GameObject go, EndNameEditAction endAction, string resourceFile)
		{
			var InstanceID = go.GetInstanceID ();
			var pathName = AssetDatabase.GetAssetPath (InstanceID);
			var icon = AssetPreview.GetMiniThumbnail (go);
			Rename (InstanceID, endAction, pathName, icon, resourceFile);
		}

		static public void Rename (int InstanceID, EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
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
