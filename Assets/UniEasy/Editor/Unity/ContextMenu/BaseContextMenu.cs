using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UniEasy.DI;

namespace UniEasy.Edit
{
	public class BaseContextMenu
	{
		protected static Dictionary<EasyMenuItemAttribute, MethodInfo> Functions = new Dictionary<EasyMenuItemAttribute, MethodInfo> ();

		[InitializeOnLoadMethod]
		static void StartInitializeOnLoadMethod ()
		{
			var group = AssemblyHelper.CSharpEditor.GetTypes ().Distinct ().ToArray ();
			for (int i = 0; i < group.Length; i++) {
				var methods = group [i].GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where (m => m.HasAttribute (typeof(EasyMenuItemAttribute))).ToArray ();
				var items = methods.Select (x => x.AllAttributes<EasyMenuItemAttribute> ().SingleOrDefault ()).ToArray ();
				for (int j = 0; j < items.Length; j++) {
					Functions.Add (items [j], methods [j]);
				}
			}
		}
	}
}
