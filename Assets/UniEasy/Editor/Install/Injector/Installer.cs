using UnityEditor;

namespace UniEasy.Edit
{
	public class Installer : ScriptAssetInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/Installer", false, 41)]
		static public void CreateInstaller ()
		{
			new Installer ().Create ();
		}

		public override string GetName ()
		{
			return "NewInstaller.cs";
		}

		public override string GetContents ()
		{
			return "using UnityEngine;"
			+ "\nusing UniEasy.DI;"
			+ "\n"
			+ "\npublic class NewInstaller : Installer<NewInstaller>"
			+ "\n{"
			+ "\n    public override void InstallBindings()"
			+ "\n    {"
			+ "\n    }"
			+ "\n}";
		}
	}
}
