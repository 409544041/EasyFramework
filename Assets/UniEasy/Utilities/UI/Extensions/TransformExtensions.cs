using UnityEngine.UI;
using UnityEngine;

namespace UniEasy
{
	public static class TransformExtensions
	{
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
