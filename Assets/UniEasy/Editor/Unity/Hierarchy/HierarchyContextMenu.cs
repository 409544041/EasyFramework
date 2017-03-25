using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UniRx;

namespace UniEasy.Edit
{
	public class HierarchyContextMenu : BaseContextMenu
	{
		private static object treeView;
		private static EditorWindow sceneHierarchyWindow;

		public static EditorWindow SceneHierarchyWindow {
			get {
				if (sceneHierarchyWindow == null) {
					sceneHierarchyWindow = EditorWindow.GetWindow (TypeHelper.SceneHierarchyWindow);
				}
				return sceneHierarchyWindow;
			}
		}

		public static object TreeView {
			get {
				if (treeView == null) {
					treeView = TypeHelper.SceneHierarchyWindow.GetProperty ("treeView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue (SceneHierarchyWindow, null);
				}
				return treeView;
			}
		}

		public static System.Action<int> ContextClickItemEvent { get; set; }

		public static System.Action ContextClickOutsideItemsEvent { get; set; }

		[InitializeOnLoadMethod]
		static void StartInitializeOnLoadMethod ()
		{
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

//			ContextClickItemEvent = (contextClickedItemID) => {
//				OnContextMenuGUI (contextClickedItemID);
//			};
//
//			ContextClickOutsideItemsEvent = () => {
//				OnContextMenuGUI ();
//			};
//
//			Observable.Timer (TimeSpan.FromSeconds (0.1f)).Subscribe (_ => {
//				
//				var contextClickItemCallbackInfo = TreeView.GetType ().GetProperty ("contextClickItemCallback");
//				var contextClickOutsideItemsCallbackInfo = TreeView.GetType ().GetProperty ("contextClickOutsideItemsCallback");
//				var contextClickItemCallback = contextClickItemCallbackInfo.GetValue (TreeView, null);
//				var contextClickOutsideItemsCallback = contextClickOutsideItemsCallbackInfo.GetValue (TreeView, null);
//
//				var contextClickItemEvent = (System.Action<int>)Delegate.Combine (ContextClickItemEvent, (System.Action<int>)contextClickItemCallback);
//				var contextClickOutsideItemsEvent = (System.Action)Delegate.Combine (ContextClickOutsideItemsEvent, (System.Action)contextClickOutsideItemsCallback);
//
//				contextClickItemCallbackInfo.SetValue (TreeView, contextClickItemEvent, null);
//				contextClickOutsideItemsCallbackInfo.SetValue (TreeView, contextClickOutsideItemsEvent, null);
//			});
		}

		static void OnHierarchyGUI (int instanceID, Rect selectionRect)
		{
			if (Event.current != null && Event.current.button == 1 && Event.current.type == EventType.mouseUp) {

				Event.current.Use ();

				GenericMenu genericMenu = new GenericMenu ();

				var contextClickedItemID = 0;
				var parametors = new object[]{ genericMenu, contextClickedItemID }; 
				var CreateGameObjectContextClick = TypeHelper.SceneHierarchyWindow.GetMethod ("CreateGameObjectContextClick", BindingFlags.Instance | BindingFlags.NonPublic);
				CreateGameObjectContextClick.Invoke (SceneHierarchyWindow, parametors);

				genericMenu.AddSeparator ("");

				var items = Functions.GetEnumerator ();
				while (items.MoveNext ()) {
					if (items.Current.Key.Name.Contains ("GameObject")) {
						genericMenu.AddItem (new GUIContent (items.Current.Key.Name), false, () => {
							items.Current.Value.Invoke (null, null);
						});
					}
				}

				if (selectionRect.Contains (Event.current.mousePosition)) {
					
				}

				genericMenu.ShowAsContext ();
			}
		}

		static void OnContextMenuGUI (int contextClickedItemID = 0)
		{
			Event.current.Use ();

			GenericMenu genericMenu = new GenericMenu ();

			genericMenu.ShowAsContext ();
		}
	}
}
