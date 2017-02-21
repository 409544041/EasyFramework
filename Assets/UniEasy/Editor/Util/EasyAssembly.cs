using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace UniEasy
{
	public class EasyAssembly
	{
		public static Type GetType (string filePath, string typeName)
		{
			Assembly assembly = Assembly.LoadFile (filePath);
			return assembly.GetType (typeName);
		}

		public static Type GetType (AssemblyName assemblyName, string typeName)
		{
			Assembly assembly = Assembly.Load (assemblyName);
			return assembly.GetType (typeName);
		}

		/// <summary>
		/// Returns the assembly that contains the c# script code for this project (currently hard coded)
		/// </summary>
		public static Assembly GetAssemblyCSharp ()
		{
			return Assembly.Load (new AssemblyName ("Assembly-CSharp"));
		}

		/// <summary>
		/// Returns the assembly that contains the unity-editor code for this project (currently hard coded)
		/// </summary>
		public static Assembly GetAssemblyCSharpEditor ()
		{
			return Assembly.Load (new AssemblyName ("Assembly-CSharp-Editor"));
		}

		public static Type GetSceneHierarchyWindow ()
		{
//		    return GetType ("c:/program files/unity/editor/data/managed/UnityEditor.dll", "UnityEditor.SceneHierarchyWindow");
			AssemblyName assemblyName = new AssemblyName ("UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			return GetType (assemblyName, "UnityEditor.SceneHierarchyWindow");
		}
	}
}
