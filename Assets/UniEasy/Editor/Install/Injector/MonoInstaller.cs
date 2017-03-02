using UnityEditor;

namespace UniEasy.Edit
{
	public class MonoInstaller : ScriptAssetInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/Mono Installer", false, 42)]
		static public void CreateMonoInstaller ()
		{
			new MonoInstaller ().Create ();
		}

		public override string GetName ()
		{
			return "NewMonoInstaller.cs";
		}

		public override string GetContents ()
		{
			return "using UnityEngine;"
			+ "\nusing UniEasy.DI;"
			+ "\n"
			+ "\npublic class NewMonoInstaller : MonoInstaller<NewMonoInstaller>"
			+ "\n{"
			+ "\n    public override void InstallBindings()"
			+ "\n    {"
			+ "\n    }"
			+ "\n}";
		}
	}
}
