using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace UniEasy.Edit
{
	public class BlockInstaller : Editor
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

		static public void ExportBlock (object obj)
		{
			EasyBlock asset = CreateBlock (obj as GameObject);
			if (asset.Count > 0)
				ScriptableObjectFactory.SaveAssetPanel<EasyBlock> (asset, asset.name);
		}

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
						Debug.LogError ("Sorry, we can't save .asset file out of assets folder!");
				}
			}
		}

		[InitializeOnLoadMethod]
		static void StartInitializeOnLoadMethod ()
		{
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
		}

		static void OnHierarchyGUI (int instanceID, Rect selectionRect)
		{
			if (Event.current != null && selectionRect.Contains (Event.current.mousePosition)
			    && Event.current.button == 1 && Event.current.type == EventType.mouseUp) {
				GameObject selectedGameObject = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
				if (selectedGameObject) {

					Event.current.Use ();

					GenericMenu genericMenu = new GenericMenu ();

					System.Type type = AssemblyHelper.GetSceneHierarchyWindow ();

					int contextClickedItemID = 0;
					object[] parametors = new object[]{ genericMenu, contextClickedItemID }; 

					MethodInfo methodInfo = type.GetMethod ("CreateGameObjectContextClick", BindingFlags.Instance | BindingFlags.NonPublic);
					Object[] objArray = Resources.FindObjectsOfTypeAll (type);
					if (objArray != null && objArray.Length > 0) {
						methodInfo.Invoke (objArray [0], parametors);
					}

					genericMenu.AddSeparator ("");
					genericMenu.AddItem (new GUIContent ("Block Installer/Export Block"), false, ExportBlock, selectedGameObject);
					genericMenu.AddItem (new GUIContent ("Block Installer/Export Block Group"), false, ExportBlockGroup, selectedGameObject);
					genericMenu.ShowAsContext ();
				}			
			}
		}
	}
}
