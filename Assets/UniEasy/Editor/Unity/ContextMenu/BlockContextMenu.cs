using UnityEditor;
﻿using UnityEngine;

namespace UniEasy.Edit
{
	public class BlockContextMenu
	{
		static public EasyBlock CreateBlock (GameObject parent)
		{
			var block = ScriptableObject.CreateInstance<EasyBlock> ();
			block.name = parent.name;
			foreach (Transform child in parent.transform) {
				var obj = new BlockObject (child.name + child.GetSiblingIndex ());
				if (PrefabUtility.GetPrefabType (child.gameObject) != PrefabType.None) {
					obj.gameObject = PrefabUtility.GetPrefabParent (child.gameObject) as GameObject;
					obj.localPosition = child.localPosition;
					obj.localRotation = child.localRotation;
					obj.localScale = child.localScale;
					block.ToDictionary ().Add (obj.blockName, obj);
				}
			}
			return block;
		}

		[EasyMenuItem ("GameObject/UniEasy/Export Block", false, 51)]
		static public void ExportBlock (object obj)
		{
			var block = CreateBlock (obj as GameObject);
			if (block.ToDictionary ().Count > 0) {
				ScriptableObjectUtility.SaveAssetPanel<EasyBlock> (block, block.name);
			}
		}

		[EasyMenuItem ("GameObject/UniEasy/Export Block Group", false, 52)]
		static public void ExportBlockGroup (object obj)
		{
			var root = obj as GameObject;
			if (root.transform.childCount > 0) {
				var path = EditorUtility.SaveFolderPanel ("Please select export folder", Application.dataPath, "Rescources");
				if (!string.IsNullOrEmpty (path)) {
					if (path.Contains (Application.dataPath)) {
						var assetPath = "";
						foreach (Transform parent in root.transform) {
							var asset = CreateBlock (parent.gameObject);
							if (asset.ToDictionary ().Count > 0) {
								assetPath = path.Replace (Application.dataPath, "Assets");
								assetPath += "/" + parent.name + parent.GetSiblingIndex () + ".asset";
								ScriptableObjectUtility.CreateAsset (asset, assetPath);
							}
						}
						ProjectWindowUtil.ShowCreatedAsset (AssetDatabase.LoadAssetAtPath<Object> (assetPath));
					} else {
						Debug.LogError ("Sorry, we cannot export file out of 'Assets' folder!");
					}
				}
			}
		}
	}
}
