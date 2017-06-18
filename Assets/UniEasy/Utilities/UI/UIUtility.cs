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

		public static Text ToConfigure (this Text text, Color32 color, string fontName = "Arial", int fontSize = 14, FontStyle fontStyle = FontStyle.Normal, TextAnchor alignment = TextAnchor.UpperLeft, bool supportRichText = true, bool raycastTarget = true)
		{
			text.font = Font.CreateDynamicFontFromOSFont (fontName, fontSize);
			text.supportRichText = supportRichText;
			text.raycastTarget = raycastTarget;
			text.fontStyle = fontStyle;
			text.alignment = alignment;
			text.fontSize = fontSize;
			text.color = color;
			return text;
		}

		public static RectTransform ToRectTransform (this Transform transform, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition)
		{
			var rectTransform = transform as RectTransform;
			if (rectTransform != null) {
				rectTransform.anchorMax = anchorMax;
				rectTransform.anchorMin = anchorMin;
				rectTransform.anchoredPosition = anchoredPosition;
				rectTransform.sizeDelta = sizeDelta;
			}
			return rectTransform;
		}
	}
}
