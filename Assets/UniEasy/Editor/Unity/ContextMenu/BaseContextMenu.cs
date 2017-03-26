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
		protected static Dictionary<EasyMenuItem, MethodInfo> Functions = new Dictionary<EasyMenuItem, MethodInfo> ();

		[InitializeOnLoadMethod]
		static internal void Steup ()
		{
			var group = AssemblyHelper.CSharpEditor.GetTypes ().Distinct ().ToArray ();
			for (int i = 0; i < group.Length; i++) {
				var methods = group [i].GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where (m => m.HasAttribute (typeof(EasyMenuItem))).ToArray ();
				var items = methods.Select (x => x.AllAttributes<EasyMenuItem> ().SingleOrDefault ()).ToArray ();
				for (int j = 0; j < items.Length; j++) {
					Functions.Add (items [j], methods [j]);
				}
			}
		}
	}
}
