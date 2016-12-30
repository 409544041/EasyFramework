using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using UniRx;

[AttributeUsage (AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class EasySingleAttribute : Attribute
{
	static public Dictionary<Type, object> singles;

	public EasySingleAttribute ()
	{

	}

	[RuntimeInitializeOnLoadMethod]
	static void OnRuntimeMethodLoad ()
	{
		#if UNITY_EDITOR
		Debug.Log ("After scene is loaded and game is running");
		Debug.Log ("Assigned fields with EasySingle Attribute");
		#endif
		singles = new Dictionary<Type, object> ();
		var assembyle = Assembly.GetExecutingAssembly ();
		var typeArray = assembyle.GetTypes ();
		for (int i = 0; i < typeArray.Length; i++) {
			var singleObjs = typeArray [i].GetCustomAttributes (typeof(EasySingleAttribute), false);
			for (int j = 0; j < singleObjs.Length; j++) {
				var single = singleObjs [j] as EasySingleAttribute;
				if (single != null && !singles.ContainsKey (typeArray [i])) {
					object value = null;
					if (typeArray [i].IsSubclassOf (typeof(MonoBehaviour))) {
						value = GameObject.FindObjectOfType (typeArray [i]);
					} else {
						#if UNITY_EDITOR
						Debug.LogError ("You cannot EasySingle Attribute to a class that does not inherit MonoBehaviour");
						Debug.LogWarning ("But you can used TODO function instead!");
						#endif
					}
					if (value != null)
						singles.Add (typeArray [i], value);
				}
			}
		}

		for (int i = 0; i < typeArray.Length; i++) {
			var fieldInfos = typeArray [i].GetFields (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			for (int j = 0; j < fieldInfos.Length; j++) {
				var fieldInfo = fieldInfos [j];
				var objects = fieldInfo.GetCustomAttributes (typeof(EasySingleAttribute), false);
				for (int k = 0; k < objects.Length; k++) {
					var single = objects [k] as EasySingleAttribute;
					if (single != null) {
						object value = null;
						if (singles.ContainsKey (fieldInfo.FieldType)) {
							value = singles [fieldInfo.FieldType];
						} else {
							if (fieldInfo.FieldType.IsSubclassOf (typeof(MonoBehaviour))) {
								var go = new GameObject (fieldInfo.FieldType.Name);
								value = go.AddComponent (fieldInfo.FieldType);
							} else {
								value = Activator.CreateInstance (fieldInfo.FieldType);
							}
							singles.Add (fieldInfo.FieldType, value);
						}
						var objs = GameObject.FindObjectsOfType (typeArray [i]);
						for (int m = 0; m < objs.Length; m++) {
							fieldInfo.SetValue (objs [m], value);
						}
					}
				}
			}
		}

		MessageBroker.Default.Receive<EasyContainer> ().Subscribe (container => {
			
		});
	}
}
