using UnityEngine;
using UnityEditor;
using System;

namespace UniEasy.Edit
{
	public class ScriptableObjectInstallerUtility
	{
		public static void Create (string path, Type type)
		{
			var go = ScriptableObject.CreateInstance (type);
			var endNameEdit = ScriptableObject.CreateInstance<EndNameEditUtility> ();
			endNameEdit.EndNameEditEvent += (instanceID, pathName, resourceFile) => {
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
