using UnityEngine;
using UnityEditor;

namespace UniEasy
{
	[CustomPropertyDrawer (typeof(MinMaxRangeAttribute))]
	public class MinMaxRangeDrawer : PropertyDrawer
	{
		private float min;
		private float max;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			var range = attribute as MinMaxRangeAttribute;

			SerializedProperty prop_min = property.FindPropertyRelative ("min");
			SerializedProperty prop_max = property.FindPropertyRelative ("max");

			min = prop_min.floatValue;
			max = prop_max.floatValue;

			label = EditorGUI.BeginProperty (position, label, property);  
			EditorGUI.BeginChangeCheck ();

			Rect contentPosition = EditorGUI.PrefixLabel (position, label);
			float f1, f2;
			if (float.TryParse (EditorGUI.TextField (new Rect (contentPosition.position, new Vector2 (0.2f * contentPosition.width, contentPosition.height)), min.ToString ("F2")), out f1)) {
				min = f1;
			}
			EditorGUI.MinMaxSlider (new Rect (contentPosition.position + new Vector2 (0.2f * contentPosition.width, 0), new Vector2 (0.6f * contentPosition.width, contentPosition.height)), ref min, ref max, range.minLimit, range.maxLimit);
			if (float.TryParse (EditorGUI.TextField (new Rect (contentPosition.position + new Vector2 (0.8f * contentPosition.width, 0), new Vector2 (0.2f * contentPosition.width, contentPosition.height)), max.ToString ("F2")), out f2)) {
				max = f2;
			}

			if (EditorGUI.EndChangeCheck ()) {
				prop_min.floatValue = min;
				prop_max.floatValue = max;
			}
			EditorGUI.EndProperty ();
		}
	}
}
