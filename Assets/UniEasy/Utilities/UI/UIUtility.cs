using UnityEngine.UI;
using UnityEngine;

namespace UniEasy
{
	public static class UIUtility
	{
		public static T Create<T> (string name, Transform parent = null) where T : Component
		{
			var go = new GameObject (name);
			go.transform.SetParent (parent);
			go.layer = LayerMask.NameToLayer ("UI");
			return go.AddComponent<T> ();
		}

		public static Toggle CreateToggle (string name, Transform parent = null)
		{
			var bg = Create<Image> (string.Format ("{0}Toggle", name), parent);
			bg.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
			var toggle = bg.gameObject.AddComponent<Toggle> ();
			var checkmark = Create<Image> (string.Format ("{0}Checkmark", name), bg.transform);
			checkmark.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
			checkmark.color = Color.white;
			toggle.graphic = checkmark;
			return toggle;
		}
	}
}
