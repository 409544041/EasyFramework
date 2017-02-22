using System.Reflection;
using System;

namespace UniEasy.Edit
{
	public class AssemblyHelper
	{
		static public Type GetType (string filePath, string typeName)
		{
			Assembly assembly = Assembly.LoadFile (filePath);
			return assembly.GetType (typeName);
		}

		static public Type GetType (AssemblyName assemblyName, string typeName)
		{
			Assembly assembly = Assembly.Load (assemblyName);
			return assembly.GetType (typeName);
		}

		static public Assembly GetAssemblyCSharp ()
		{
			return Assembly.Load (new AssemblyName ("Assembly-CSharp"));
		}

		static public Assembly GetAssemblyCSharpEditor ()
		{
			return Assembly.Load (new AssemblyName ("Assembly-CSharp-Editor"));
		}

		static public Type GetSceneHierarchyWindow ()
		{
//		    return GetType ("c:/program files/unity/editor/data/managed/UnityEditor.dll", "UnityEditor.SceneHierarchyWindow");
			AssemblyName assemblyName = new AssemblyName ("UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			return GetType (assemblyName, "UnityEditor.SceneHierarchyWindow");
		}
	}
}
