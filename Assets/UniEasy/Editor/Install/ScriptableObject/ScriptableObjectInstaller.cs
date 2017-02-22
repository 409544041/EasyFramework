using UnityEngine;
using UnityEditor;
using System;

namespace UniEasy.Edit
{
	/// <summary>
	/// A helper class for instantiating ScriptableObjects in the editor.
	/// </summary>
	public class ScriptableObjectInstaller
	{
		public static void Create (string path, Type type)
		{
			var go = ScriptableObject.CreateInstance (type);
			var endNameEdit = ScriptableObject.CreateInstance<EndNameEditUtil> ();
			endNameEdit.EndAction += (instanceID, pathName, resourceFile) => {
				AssetDatabase.CreateAsset (EditorUtility.InstanceIDToObject (instanceID), AssetDatabase.GenerateUniqueAssetPath (pathName));
			};
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists (
				go.GetInstanceID (),
				endNameEdit,
				string.Format ("{0}.asset", path),
				EditorGUIUtility.IconContent ("ScriptableObject Icon", "").image as Texture2D,
				"");
		}
	}
}
