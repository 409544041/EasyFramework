using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CanEditMultipleObjects, CustomEditor (typeof(Transform), true)]
public class EasyTransform : DecoratorEditor
{
	Dictionary<string, SerializedProperty> SerializedProperties;

	public EasyTransform () : base ("TransformInspector")
	{
	}

	void OnEnable ()
	{
		SerializedProperties = new Dictionary<string, SerializedProperty> ();
		SerializedProperties.Add ("m_LocalPosition", serializedObject.FindProperty ("m_LocalPosition"));
		SerializedProperties.Add ("m_LocalRotation", serializedObject.FindProperty ("m_LocalRotation"));
		SerializedProperties.Add ("m_LocalScale", serializedObject.FindProperty ("m_LocalScale"));
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		serializedObject.Update ();
		DrawCustomInspector ();
		serializedObject.ApplyModifiedProperties ();
	}

	void DrawCustomInspector ()
	{
		GUILayoutOption option = GUILayout.MinWidth (82f);
		GUILayout.BeginHorizontal ();
		{
			bool reset = GUILayout.Button ("Reset", option);
			bool copy = GUILayout.Button ("Copy", option);
			bool paste = GUILayout.Button ("Paste", option);

			if (reset) {
				SerializedProperties ["m_LocalPosition"].vector3Value = Vector3.zero;
				SerializedProperties ["m_LocalRotation"].quaternionValue = Quaternion.identity;
				SerializedProperties ["m_LocalScale"].vector3Value = Vector3.one;
			}
			if (copy) {
				ComponentUtility.CopyComponent (serializedObject.targetObject as Component);
			}
			if (paste) {
				foreach (var targetObject in serializedObject.targetObjects) {
					ComponentUtility.PasteComponentValues (targetObject as Component);
				}
			}
		}
		GUILayout.EndHorizontal ();
	}
}
