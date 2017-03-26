using UnityEngine;
using UnityEditor;

namespace UniEasy.Edit
{
	public class BlockContextMenu
	{
		static public EasyBlock CreateBlock (GameObject parent)
		{
			EasyBlock asset = ScriptableObject.CreateInstance<EasyBlock> ();
			asset.name = parent.name;
			foreach (Transform child in parent.transform) {
				BlockObject block = new BlockObject (child.name + child.GetSiblingIndex ());
				if (PrefabUtility.GetPrefabType (child.gameObject) != PrefabType.None) {
					block.gameObject = PrefabUtility.GetPrefabParent (child.gameObject) as GameObject;
					block.localPosition = child.localPosition;
					block.localRotation = child.localRotation;
					block.localScale = child.localScale;
				}
				if (block.gameObject != null) {
					asset.Add (block);
				}
			}
			return asset;
		}

		[EasyMenuItem ("GameObject/UniEasy/Export Block", false, 51)]
		static public void ExportBlock (object obj)
		{
			EasyBlock asset = CreateBlock (obj as GameObject);
			if (asset.Count > 0)
				ScriptableObjectFactory.SaveAssetPanel<EasyBlock> (asset, asset.name);
		}

		[EasyMenuItem ("GameObject/UniEasy/Export Block Group", false, 52)]
		static public void ExportBlockGroup (object obj)
		{
			var root = obj as GameObject;
			if (root.transform.childCount > 0) {
				string path = EditorUtility.SaveFolderPanel ("Select export .asset file's folder", Application.dataPath, "Rescources");
				if (!string.IsNullOrEmpty (path)) {
					if (path.Contains (Application.dataPath)) {
						string assetPath = "";
						foreach (Transform parent in root.transform) {
							EasyBlock asset = CreateBlock (parent.gameObject);
							if (asset.Count > 0) {
								assetPath = path.Replace (Application.dataPath, "Assets");
								assetPath += "/" + parent.name + parent.GetSiblingIndex () + ".asset";
								ScriptableObjectFactory.CreateAsset (asset, assetPath);
							}
						}
						ProjectWindowUtil.ShowCreatedAsset (AssetDatabase.LoadAssetAtPath<Object> (assetPath));
					} else
						Debugger.LogError ("Sorry, we can't save .asset file out of assets folder!");
				}
			}
		}
	}
}
