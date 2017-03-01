using UnityEditor;

namespace UniEasy.Edit
{
	public class ScriptableObjectInstaller : ScriptAssetInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/ScriptableObject Installer", false, 43)]
		static public void CreateScriptableObjectInstaller ()
		{
			new ScriptableObjectInstaller ().Create ();
		}

		public override string GetName ()
		{
			return "NewScriptableObjectInstaller.cs";
		}

		public override string GetContents ()
		{
			return "using UnityEngine;"
			+ "\nusing UniEasy.DI;"
			+ "\n"
			+ "\npublic class NewScriptableObjectInstaller : ScriptableObjectInstaller<NewScriptableObjectInstaller>"
			+ "\n{"
			+ "\n    public override void InstallBindings()"
			+ "\n    {"
			+ "\n    }"
			+ "\n}";
		}
	}
}
