using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace UniEasy.Edit
{
	public class ScriptAssetUlit
	{
		[MenuItem ("Assets/Create/UniEasy/Fast CSharp Installer", false, 1)]
		public static void CreateScriptAsset ()
		{
			CreateScriptAsset ("NewScriptAsset",
				"using UniEasy;" +
				"\n" +
				"\npublic class NewScriptAsset" +
				"\n{" +
				"\n" +
				"\n}");
		}

		public static void CreateScriptAsset (string name, string contents)
		{
			var assetPath = AssetDatabase.GetAssetPath (Selection.activeInstanceID) +
			                "/" + name + ".cs";

			var endNameEdit = ScriptableObject.CreateInstance<EndNameEdit> ();
			endNameEdit.EndAction += (instanceID, pathName, resourceFile) => {
				contents = contents.Replace (name, Path.GetFileNameWithoutExtension (pathName));
				File.WriteAllText (pathName, contents);
				AssetDatabase.Refresh ();
			};

			StartNameEditor.Create (
				0,
				endNameEdit,
				assetPath,
				EditorGUIUtility.IconContent ("cs Script Icon", "").image as Texture2D,
				"");
		}
	}
}
