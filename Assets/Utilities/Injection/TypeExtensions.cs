using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

static public class TypeExtensions
{
	// Returns all instance fields, including private and public and also those in base classes
	public static IEnumerable<FieldInfo> GetAllInstanceFields (this Type type)
	{
		foreach (var fieldInfo in type.DeclaredInstanceFields()) {
			yield return fieldInfo;
		}

		if (type.BaseType () != null && type.BaseType () != typeof(object)) {
			foreach (var fieldInfo in type.BaseType().GetAllInstanceFields()) {
				yield return fieldInfo;
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

	public static bool HasAttribute (
		this MemberInfo provider, params Type[] attributeTypes)
	{
		return provider.AllAttributes (attributeTypes).Any ();
	}

	public static IEnumerable<Attribute> AllAttributes (
		this MemberInfo provider, params Type[] attributeTypes)
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
