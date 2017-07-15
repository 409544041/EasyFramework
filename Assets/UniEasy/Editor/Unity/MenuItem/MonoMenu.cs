using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UniEasy.Edit
{
	public class MonoMenu
	{
		[MenuItem ("Help/About MonoDevelop...")]
		public static void GetVersion ()
		{
			var type = TypeHelper.MonoRuntimeType;
			if (type != null) {                                          
				var displayName = type.GetMethod ("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
				if (displayName != null) {
					Debug.Log (displayName.Invoke (null, null));
				}
			}
		}
	}
}
