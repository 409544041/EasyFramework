using UnityEngine;
using UnityEditor;
using System.Reflection;

public class BlockFactoryEditor : Editor
{
	static void CreateBlock (object obj)
	{
		var parent = obj as GameObject;
		EasyBlock asset = ScriptableObject.CreateInstance<EasyBlock> ();
		asset.name = parent.name;
		foreach (Transform child in parent.transform) {
			BlockObject block = new BlockObject (child.name);
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
		ScriptableObjectFactory.SaveAssetPanel<EasyBlock> (asset, asset.name);
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

				System.Type type = EasyAssembly.GetSceneHierarchyWindow ();

				int contextClickedItemID = 0;
				object[] parametors = new object[]{ genericMenu, contextClickedItemID }; 

				MethodInfo methodInfo = type.GetMethod ("CreateGameObjectContextClick", BindingFlags.Instance | BindingFlags.NonPublic);
				Object[] objArray = Resources.FindObjectsOfTypeAll (type);
				if (objArray != null && objArray.Length > 0) {
					methodInfo.Invoke (objArray [0], parametors);
				}

				genericMenu.AddSeparator ("");
				genericMenu.AddItem (new GUIContent ("BlockFactory/Create Block"), false, CreateBlock, selectedGameObject);
				genericMenu.ShowAsContext ();
			}			
		}
	}
}
