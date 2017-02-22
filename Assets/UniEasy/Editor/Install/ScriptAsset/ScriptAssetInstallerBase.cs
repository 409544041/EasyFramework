using UnityEngine;

namespace UniEasy.Edit
{
	public abstract class ScriptAssetInstallerBase : IScriptAssetInstaller
	{
		abstract public void Create ();

		abstract public string GetName ();

		abstract public string GetContents ();

		abstract public Texture2D GetIcon ();
	}
}
