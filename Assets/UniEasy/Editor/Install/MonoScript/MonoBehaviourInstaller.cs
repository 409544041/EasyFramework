using UnityEditor;

namespace UniEasy.Edit
{
	public class MonoBehaviourInstaller : ScriptAssetInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/MonoBehaviour Installer", false, 9)]
		static public void CreateMonoBehaviour ()
		{
			new MonoBehaviourInstaller ().Create ();
		}

		public override string GetName ()
		{
			return "NewBehaviourScript.cs";
		}

		public override string GetContents ()
		{
			return "using UnityEngine;" +
			"\nusing System.Collections;" +
			"\n" +
			"\npublic class NewBehaviourScript : MonoBehaviour {" +
			"\n" +
			"\n\tvoid Start () {" +
			"\n\t" +
			"\n\t}" +
			"\n\t" +
			"\n\tvoid Update () {" +
			"\n\t" +
			"\n\t}" +
			"\n}";
		}
	}
}
