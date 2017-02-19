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

		public static Type GetSceneHierarchyWindow ()
		{
//		    return GetType ("c:/program files/unity/editor/data/managed/UnityEditor.dll", "UnityEditor.SceneHierarchyWindow");
			AssemblyName assemblyName = new AssemblyName ("UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			return GetType (assemblyName, "UnityEditor.SceneHierarchyWindow");
		}
	}
}
