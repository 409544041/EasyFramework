﻿using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

namespace UniEasy.Edit
{
	public class SearchSceneMissingContextMenu
	{
		private static PropertyInfo treeView;
		private static FieldInfo searchFilter;
		private static MethodInfo searchChanged;
		private static EditorWindow sceneHierarchyWindow;

		public static EditorWindow SceneHierarchyWindowObject {
			get {
				if (sceneHierarchyWindow == null) {
					sceneHierarchyWindow = EditorWindow.GetWindow (TypeHelper.SceneHierarchyWindow, false, "Hierarchy", true);
				}
				return sceneHierarchyWindow;
			}
		}

		public static string SearchFilter {
			get {
				if (searchFilter == null) {
					searchFilter = TypeHelper.SceneHierarchyWindow.GetField ("m_SearchFilter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				}
				return searchFilter.GetValue (SceneHierarchyWindowObject).ToString ();
			}
			set {
				if (searchFilter == null) {
					searchFilter = TypeHelper.SceneHierarchyWindow.GetField ("m_SearchFilter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				}
				searchFilter.SetValue (SceneHierarchyWindowObject, value);
			}
		}

		public static object TreeView {
			get {
				if (treeView == null) {
					treeView = TypeHelper.SceneHierarchyWindow.GetProperty ("treeView", BindingFlags.NonPublic | BindingFlags.Instance);
				}
				return treeView.GetValue (SceneHierarchyWindowObject, null);
			}
		}

		public static object Data {
			get {
				return TreeView.GetType ().GetProperty ("data").GetValue (TreeView, null);
			}
		}

		public static FieldInfo Rows {
			get {
				#if UNITY_5_5_OR_NEWER
				return Data.GetType ().GetField ("m_Rows", BindingFlags.NonPublic | BindingFlags.Instance);
				#else
				return Data.GetType ().GetField ("m_VisibleRows", BindingFlags.NonPublic | BindingFlags.Instance);
				#endif
			}
		}

		public static FieldInfo RowCount {
			get {
				return Data.GetType ().GetField ("m_RowCount", BindingFlags.NonPublic | BindingFlags.Instance);
			}
		}

		public static MethodInfo SearchChanged {
			get {
				if (searchChanged == null) {
					searchChanged = TypeHelper.SceneHierarchyWindow.GetMethod ("SearchChanged");
				}
				return searchChanged;
			}
		}

		[EasyMenuItem ("GameObject/UniEasy/Search for All Missing Components", false, 1)]
		public static void SearchForAllMissingComponents ()
		{
			SceneHierarchyWindowObject.Show ();
			SearchFilter = "t: Component";
			SearchChanged.Invoke (SceneHierarchyWindowObject, null);
			SearchProcess (go => {
				return HaveMissingComponent (go);
			});
			SceneHierarchyWindowObject.Repaint ();
		}

		static void SearchProcess (Func<GameObject, bool> e)
		{
			var list = new List<object> ();
			var items = (Rows.GetValue (Data) as IEnumerable).GetEnumerator ();
			var itemType = items.GetType ().GetGenericArguments () [0];
			var itemIdPropertyInfo = itemType.GetProperty ("id", BindingFlags.Public | BindingFlags.Instance);
			var activeId = Selection.activeInstanceID;
			while (items.MoveNext ()) {
				var id = (int)itemIdPropertyInfo.GetValue (items.Current, null);
				Selection.activeInstanceID = id;
				if (e (Selection.activeGameObject)) {
					list.Add (items.Current);
				}
			}
			Selection.activeInstanceID = activeId;
			var itemList = Activator.CreateInstance (typeof(List<>).MakeGenericType (new Type[] {
				itemType,
			}));
			var addMethod = itemList.GetType ().GetMethod ("Add");
			for (int i = 0; i < list.Count; i++) {
				addMethod.Invoke (itemList, new object [] {
					list [i],
				});
			}
			Rows.SetValue (Data, itemList);
			RowCount.SetValue (Data, list.Count);
		}

		static bool HaveMissingComponent (GameObject go)
		{
			if (go != null) {
				var components = go.GetComponents<Component> ();
				for (int i = 0; i < components.Length; i++) {
					if (components [i] == null) {
						return true;
					}
				}
			}
			return false;
		}

		static GameObject[] FindMissingComponents (GameObject[] gameObjects)
		{
			var result = new List<GameObject> (); 
			for (int i = 0; i < gameObjects.Length; i++) {
				if (HaveMissingComponent (gameObjects [i])) {
					result.Add (gameObjects [i]);
				}
			}
			return result.ToArray ();
		}

		[EasyMenuItem ("GameObject/UniEasy/Clean Selected Missing Components", false, 2)]
		static void CleanSelectedMissingComponents ()
		{
			CleanupMissingComponents<Component> (Selection.gameObjects);
		}

		[EasyMenuItem ("GameObject/UniEasy/Clean Selected Missing Components", true, 2)]
		static bool CheckSelectedGameObjectsInSceneNotEmpty ()
		{
			if (Selection.gameObjects.Length > 0) {
				return true;
			}
			return false;
		}

		static void CleanupMissingComponents<T> (GameObject[] gameObjects)
		{
			for (int i = 0; i < gameObjects.Length; i++) {
				var components = gameObjects [i].GetComponents<T> ();
				var serializedObject = new SerializedObject (gameObjects [i]);
				var prop = serializedObject.FindProperty ("m_Component");
				int r = 0;
				for (int j = 0; j < components.Length; j++) {
					if (components [j] == null) {
						prop.DeleteArrayElementAtIndex (j - r);
						r++;
					}
				}
				serializedObject.ApplyModifiedProperties ();
			}
		}
	}
}
