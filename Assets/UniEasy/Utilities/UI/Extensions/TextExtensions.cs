using UnityEngine.UI;
using UnityEngine;

namespace UniEasy
{
	public static class TextExtensions
	{
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
	}
}
