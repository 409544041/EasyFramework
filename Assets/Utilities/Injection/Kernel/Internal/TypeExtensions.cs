using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace UniEasy
{
	static public class TypeExtensions
	{
		public static bool DerivesFrom<T> (this Type that)
		{
			return DerivesFrom (that, typeof(T));
		}

		// This seems easier to think about than IsAssignableFrom
		public static bool DerivesFrom (this Type right, Type left)
		{
			return left != right && right.DerivesFromOrEqual (left);
		}

		public static bool DerivesFromOrEqual<T> (this Type that)
		{
			return DerivesFromOrEqual (that, typeof(T));
		}

		public static bool DerivesFromOrEqual (this Type right, Type left)
		{
			return left == right || left.IsAssignableFrom (right);
		}

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

		public static IEnumerable<T> AllAttributes<T> (this MemberInfo provider) where T : Attribute
		{
			return provider.AllAttributes (typeof(T)).Cast<T> ();
		}

		public static IEnumerable<Attribute> AllAttributes (this MemberInfo provider, params Type[] attributeTypes)
		{
			var allAttributes = provider.GetCustomAttributes (true).Cast<Attribute> ();

			if (attributeTypes.Length == 0) {
				return allAttributes;
			}

			return allAttributes.Where (a => attributeTypes.Any (x => a.GetType ().DerivesFromOrEqual (x)));
		}

		public static IEnumerable<T> AllAttributes<T> (this ParameterInfo provider) where T : Attribute
		{
			return provider.AllAttributes (typeof(T)).Cast<T> ();
		}

		public static IEnumerable<Attribute> AllAttributes (this ParameterInfo provider, params Type[] attributeTypes)
		{
			var allAttributes = provider.GetCustomAttributes (true).Cast<Attribute> ();

			if (attributeTypes.Length == 0) {
				return allAttributes;
			}

			return allAttributes.Where (a => attributeTypes.Any (x => a.GetType ().DerivesFromOrEqual (x)));
		}

		public static bool IsValueType (this Type type)
		{
			return type.IsValueType;
		}

		public static ConstructorInfo[] Constructors (this Type type)
		{
			return type.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		// Returns all instance methods, including private and public and also those in base classes
		public static IEnumerable<MethodInfo> GetAllInstanceMethods (this Type type)
		{
			foreach (var methodInfo in type.DeclaredInstanceMethods()) {
				yield return methodInfo;
			}

			if (type.BaseType () != null && type.BaseType () != typeof(object)) {
				foreach (var methodInfo in type.BaseType().GetAllInstanceMethods()) {
					yield return methodInfo;
				}
			}
		}

		public static MethodInfo[] DeclaredInstanceMethods (this Type type)
		{
			return type.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public static IEnumerable<Type> GetParentTypes (this Type type)
		{
			if (type == null || type.BaseType () == null || type == typeof(object) || type.BaseType () == typeof(object)) {
				yield break;
			}

			yield return type.BaseType ();

			foreach (var ancestor in type.BaseType().GetParentTypes()) {
				yield return ancestor;
			}
		}

		public static object GetDefaultValue (this Type type)
		{
			if (type.IsValueType ()) {
				return Activator.CreateInstance (type);
			}

			return null;
		}

		public static bool IsOpenGenericType (this Type type)
		{
			return type.IsGenericType () && type == type.GetGenericTypeDefinition ();
		}

		public static bool IsGenericType (this Type type)
		{
			return type.IsGenericType;
		}
	}
}
