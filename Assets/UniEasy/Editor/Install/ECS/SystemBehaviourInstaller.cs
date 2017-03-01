using UnityEditor;

namespace UniEasy.Edit
{
	public class SystemBehaviourInstaller : ScriptAssetInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/SystemBehaviour Installer", false, 22)]
		static public void CreateSystemBehaviour ()
		{
			new SystemBehaviourInstaller ().Create ();
		}

		public override string GetName ()
		{
			return "NewSystemBehaviourInstaller.cs";
		}

		public override string GetContents ()
		{
			return "using UnityEngine;" +
			"\nusing UniEasy.ECS;" +
			"\n" +
			"\npublic class NewSystemBehaviourInstaller : SystemBehaviour" +
			"\n{" +
			"\n\tprotected override void Awake ()" +
			"\n\t{" +
			"\n\t\tbase.Awake ();" +
			"\n\t}" +
			"\n" +
			"\n\tvoid Start ()" +
			"\n\t{" +
			"\n\t\t" +
			"\n\t}" +
			"\n}";
		}
	}
}
