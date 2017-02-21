namespace UniEasy.Edit
{
	public class MonoBehaviourInstaller : ScriptAssetInstallerBase
	{
		public override string GetScriptAssetName ()
		{
			return "NewBehaviourScript";
		}

		public override string GetScriptAssetContents ()
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
