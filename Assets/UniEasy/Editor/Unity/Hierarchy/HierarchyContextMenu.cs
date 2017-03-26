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
		static void StartSteup ()
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

				var items = Functions.OrderBy (x => x.Key.priority).GetEnumerator ();
				var separator = 50;
				while (items.MoveNext ()) {
					var attribute = items.Current.Key;
					var function = items.Current.Value;
					if (attribute.menuItem.Contains ("GameObject/")) {
						var itemName = attribute.menuItem.Replace ("GameObject/", "");
						if (attribute.priority > separator) {
							genericMenu.AddSeparator (itemName.Substring (0, itemName.LastIndexOf ("/") + 1));
							separator += 50;
						}
						var result = true;
						if (attribute.validate) {
							result = (bool)function.Invoke (null, null);
						}

						var param = function.GetParameters ();
						if (param.Length > 1) {
							result = false;
						} else if (param.Length == 1) {
							result = false;
							if (param [0].ParameterType == typeof(object) && Selection.activeGameObject != null) {

								genericMenu.AddItem (new GUIContent (itemName), false, (go) => {
									function.Invoke (null, new object[] { go });
								}, Selection.activeGameObject);

								result = true;
								continue;
							}
						}

						if (result) {
							genericMenu.AddItem (new GUIContent (itemName), false, () => {
								function.Invoke (null, null);
							});
						} else {
							genericMenu.AddDisabledItem (new GUIContent (itemName));
						}
					}
				}

				genericMenu.AddSeparator ("");

				var contextClickedItemID = 0;
				var parametors = new object[]{ genericMenu, contextClickedItemID }; 
				var CreateGameObjectContextClick = TypeHelper.SceneHierarchyWindow.GetMethod ("CreateGameObjectContextClick", BindingFlags.Instance | BindingFlags.NonPublic);
				CreateGameObjectContextClick.Invoke (SceneHierarchyWindow, parametors);

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
