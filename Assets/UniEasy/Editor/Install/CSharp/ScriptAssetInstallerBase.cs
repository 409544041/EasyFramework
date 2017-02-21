namespace UniEasy.Edit
{
	public abstract class ScriptAssetInstallerBase : IScriptAssetInstaller
	{
		abstract public string GetScriptAssetName ();

		abstract public string GetScriptAssetContents ();
	}
}
