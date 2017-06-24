using UnityEngine.UI;
using UnityEngine;

namespace UniEasy.Console
{
	public struct DebugLine
	{
		public LogType type;
		public Text output;
		public Text outputTimes;
		public Image outputBackground;
		public Image outputTimesBackground;

		public DebugLine (LogType type, Transform parent)
		{
			this.type = type;

			var og = UIUtility.Create<RectTransform> ("OutputBackground", parent);
			og.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
			og.gameObject.AddComponent<HorizontalLayoutGroup> ().padding = new RectOffset (0, 0, 5, 5);
			outputBackground = og.gameObject.AddComponent<Image> ();
			outputBackground.raycastTarget = false;
			outputBackground.color = new Color32 (0x00, 0x00, 0x00, 0x48);
			var op = UIUtility.Create<RectTransform> ("Output", og);
			op.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
			output = op.gameObject.AddComponent<Text> ();
			output.ToConfigure (Color.white, fontSize: 20, raycastTarget: false);

			var tg = UIUtility.Create<RectTransform> ("OutputTimesBackground", op);
			tg.ToRectTransform (new Vector2 (1, 0.5f), new Vector2 (1, 0.5f), new Vector2 (25, 20), new Vector2 (-40, 0));
			outputTimesBackground = tg.gameObject.AddComponent<Image> ();
			DefaultControls.CreateSlider (new DefaultControls.Resources ());

//			outputTimesBackground.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite> ("UI/Skin/Knob.psd");
			outputTimesBackground.color = new Color32 (0x42, 0x42, 0x42, 0x80);
			outputTimesBackground.raycastTarget = false;
			var ot = UIUtility.Create<RectTransform> ("OutputTimes", tg);
			ot.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
			outputTimes = ot.gameObject.AddComponent<Text> ();
			outputTimes.ToConfigure (Color.white, raycastTarget: false, alignment: TextAnchor.MiddleCenter);
			outputTimes.horizontalOverflow = HorizontalWrapMode.Overflow;
			outputTimes.alignByGeometry = true;
		}
	}
}
