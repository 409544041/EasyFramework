using UnityEngine;
using UnityEditor;
using System.Linq;

namespace UniEasy.Edit
{
	/// <summary>
	/// A helper class for instantiating ScriptableObjects in the editor.
	/// </summary>
	public class ScriptableObjectInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/ScriptableObject Installer")]
		public static void CreateScriptableObject ()
		{
			var assembly = EasyAssembly.GetAssemblyCSharp ();
			
			// Get all classes derived from ScriptableObject
			var allScriptableObjects = (from t in assembly.GetTypes ()
			                            where t.IsSubclassOf (typeof(ScriptableObject))
			                            where !t.IsGenericType
			                            select t).ToArray ();

			// Show the selection window.
			ScriptableObjectWindow.Init (allScriptableObjects);
		}
	}
}
