using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace UniEasy.Edit
{
	/// <summary>
	/// A helper class for instantiating MonoBehaviour Class in the editor.
	/// </summary>
	public class MonoBehaviourInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/MonoBehaviour Installer")]
		public static void CreateMonoBehaviour ()
		{
			var path = string.Concat (AssetDatabase.GetAssetPath (Selection.activeInstanceID),
				           "/NewBehaviourScript.cs");
			var contents = 
				"using UnityEngine;" +
				"\n" +
				"\npublic class NewBehaviourScript : MonoBehaviour" +
				"\n{" +
				"\n" +
				"\n\tvoid Start ()" +
				"\n\t{" +
				"\n\t" +
				"\n\t}" +
				"\n}";

			var endNameEdit = ScriptableObject.CreateInstance<EndNameEdit> ();
			endNameEdit.EndAction += (instanceID, pathName, resourceFile) => {
				contents = contents.Replace ("NewBehaviourScript", Path.GetFileNameWithoutExtension (pathName));
				File.WriteAllText (pathName, contents);
				AssetDatabase.Refresh ();
			};

			StartNameEditor.Create (new Object ().GetInstanceID (), endNameEdit, path,
				AssetPreview.GetMiniTypeThumbnail (typeof(MonoBehaviour)), "");
		}
	}
}
