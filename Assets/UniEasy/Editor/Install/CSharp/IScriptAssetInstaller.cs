using UnityEngine;

namespace UniEasy.Edit
{
	public interface IScriptAssetInstaller
	{
		void Create ();

		string GetName ();

		string GetContents ();

		Texture2D GetIcon ();
	}
}
