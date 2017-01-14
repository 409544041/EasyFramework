using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace UniEasy
{
	public class TypeAnalyzer
	{
		static Dictionary<Type, EasyInjectInfo> typeInfo = new Dictionary<Type, EasyInjectInfo> ();

		static public EasyInjectInfo GetInfo<T> ()
		{
			var type = typeof(T);
			if (typeInfo.ContainsKey (type)) {
				return typeInfo [type];
			}
			return null;
		}

		static public EasyInjectInfo GetInfo (Type type)
		{
			EasyInjectInfo info;
			if (!typeInfo.TryGetValue (type, out info)) {
				info = CreateTypeInfo (type);
				typeInfo.Add (type, info);
			}
			return info;
		}

		static EasyInjectInfo CreateTypeInfo (Type type)
		{
			return new EasyInjectInfo (
				GetFieldInjectables (type).ToList (),
				GetPropertyInjectables (type).ToList ()
			);
		}

		static IEnumerable<InjectableInfo> GetFieldInjectables (Type type)
		{
			var fieldInfos = type.GetAllInstanceFields ()
				.Where (x => x.HasAttribute (typeof(InjectAttribute))).ToArray ();

			for (int i = 0; i < fieldInfos.Length; i++) {
				yield return CreateForMember (fieldInfos [i], type);
			}
		}

		static IEnumerable<InjectableInfo> GetPropertyInjectables (Type type)
		{
			var propInfos = type.GetAllInstanceProperties ()
				.Where (x => x.HasAttribute (typeof(InjectAttribute))).ToArray ();

			for (int i = 0; i < propInfos.Length; i++) {
				yield return CreateForMember (propInfos [i], type);
			}
		}

		static InjectableInfo CreateForMember (MemberInfo memberInfo, Type parentType)
		{
			Type memberType;
			Action<object, object> setter;
			if (memberInfo is FieldInfo) {
				var fieldInfo = memberInfo as FieldInfo;
				setter = ((object injectable, object value) => fieldInfo.SetValue (injectable, value));
				memberType = fieldInfo.FieldType;
			} else {
				var propInfo = memberInfo as PropertyInfo;
				setter = ((object injectable, object value) => propInfo.SetValue (injectable, value, null));
				memberType = propInfo.PropertyType;
			}
			return new InjectableInfo (memberType, setter);
		}
	}
}
