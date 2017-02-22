using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace UniEasy.Edit
{
	public class ProjectWindowUtilExtensions
	{
		// empty array for invoking methods using reflection
		static private readonly object[] EMPTY_ARRAY = new object[0];
		static private Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo> ();
		static private ProjectWindowUtil instance = null;

		static public ProjectWindowUtil Instance {
			get {
				return instance;
			}
		}

		static protected object CallMethod (string methodName)
		{
			MethodInfo method = null;

			// Add MethodInfo to cache
			if (!methods.ContainsKey (methodName)) {
				var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

				method = typeof(ProjectWindowUtil).GetMethod (methodName, flags);

				if (method != null) {
					methods [methodName] = method;
				} else {
					Debug.LogError (string.Format ("Could not find method {0}", method));
				}
			} else {
				method = methods [methodName];
			}

			if (method != null) {
				return method.Invoke (Instance, EMPTY_ARRAY);
			}
			return default (object);
		}

		static public string GetActiveFolderPath ()
		{
			return CallMethod ("GetActiveFolderPath").ToString ();
		}
	}
}
