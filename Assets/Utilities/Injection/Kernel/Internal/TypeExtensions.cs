using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace UniEasy
{
	static public class TypeExtensions
	{
		// Returns all instance fields, including private and public and also those in base classes
		public static IEnumerable<FieldInfo> GetAllInstanceFields (this Type type)
		{
			var fieldInfos = type.DeclaredInstanceFields ();
			for (int i = 0; i < fieldInfos.Length; i++) {
				yield return fieldInfos [i];
			}

			if (type.BaseType () != null && type.BaseType () != typeof(object)) {
				var baseFieldInfos = type.BaseType ().GetAllInstanceFields ().ToArray ();
				for (int i = 0; i < baseFieldInfos.Length; i++) {
					yield return baseFieldInfos [i];
				}
			}
		}

		public static FieldInfo[] DeclaredInstanceFields (this Type type)
		{
			return type.GetFields (
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public static Type BaseType (this Type type)
		{
			return type.BaseType;
		}

		// Returns all instance properties, including private and public and also those in base classes
		public static IEnumerable<PropertyInfo> GetAllInstanceProperties (this Type type)
		{
			var propInfos = type.DeclaredInstanceProperties ();
			for (int i = 0; i < propInfos.Length; i++) {
				yield return propInfos [i];
			}

			if (type.BaseType () != null && type.BaseType () != typeof(object)) {
				var basePropInfos = type.BaseType ().GetAllInstanceProperties ().ToArray ();
				for (int i = 0; i < basePropInfos.Length; i++) {
					yield return basePropInfos [i];
				}
			}
		}

		public static PropertyInfo[] DeclaredInstanceProperties (this Type type)
		{
			return type.GetProperties (
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public static bool HasAttribute (this MemberInfo provider, params Type[] attributeTypes)
		{
			return provider.AllAttributes (attributeTypes).Any ();
		}

		public static bool HasAttribute<T> (this MemberInfo provider) where T : Attribute
		{
			return provider.AllAttributes (typeof(T)).Any ();
		}

		public static IEnumerable<Attribute> AllAttributes (this MemberInfo provider, params Type[] attributeTypes)
		{
			var allAttributes = provider.GetCustomAttributes (true).Cast<Attribute> ();

			if (attributeTypes.Length == 0) {
				return allAttributes;
			}

			return allAttributes.Where (a => attributeTypes.Any (x => a.GetType ().DerivesFromOrEqual (x)));
		}

		public static bool DerivesFromOrEqual (this Type a, Type b)
		{
			return b == a || b.IsAssignableFrom (a);
		}
	}
}
