using UnityEngine;
using UnityEditor;
using System.IO;

namespace UniEasy.Edit
{
	public class ScriptAssetInstaller : ScriptAssetInstallerBase
	{
		[MenuItem ("Assets/Create/UniEasy/Fast Script Installer", false, 1)]
		static public void CreateScriptAsset ()
		{
			new ScriptAssetInstaller ().Create ();
		}

		public override void Create ()
		{
			var path = ProjectWindowUtilExtensions.GetActiveFolderPath () + "/" + GetName ();
						
			var endNameEdit = ScriptableObject.CreateInstance<EndNameEditUtil> ();
			endNameEdit.EndAction += (instanceID, pathName, resourceFile) => {
				var contents = GetContents ().Replace (
					               Path.GetFileNameWithoutExtension (GetName ()), 
					               Path.GetFileNameWithoutExtension (pathName));
				File.WriteAllText (pathName, contents);
				AssetDatabase.Refresh ();
			};
			
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists (
				0,
				endNameEdit,
				path,
				GetIcon (),
				"");
		}

		public override string GetName ()
		{
			return "NewScriptAsset.cs";
		}

		public override string GetContents ()
		{
			return "using UniEasy;" +
			"\n" +
			"\npublic class NewScriptAsset" +
			"\n{" +
			"\n" +
			"\n}";
		}

		public override Texture2D GetIcon ()
		{
			return EditorGUIUtility.IconContent ("cs Script Icon", "").image as Texture2D;
		}
	}
}
