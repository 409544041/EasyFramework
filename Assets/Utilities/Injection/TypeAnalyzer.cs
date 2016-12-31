using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UniRx;

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
			return new EasyInjectInfo ();
		}

		static public void GetInfo (object entity)
		{
			var type = entity.GetType ();
			var fieldInjectables = GetFieldInjectables (type).ToList ();
			var inject = new EasyInjectInfo (entity, fieldInjectables);
			typeInfo.Add (type, inject);
			MessageBroker.Default.Publish (inject);
		}

		static IEnumerable<InjectableInfo> GetFieldInjectables (Type type)
		{
			var fieldInfos = type.GetAllInstanceFields ()
				.Where (x => x.HasAttribute (typeof(InjectAttribute)));

			foreach (var fieldInfo in fieldInfos) {
				yield return CreateForMember (fieldInfo, type);
			}
		}

		static InjectableInfo CreateForMember (MemberInfo memberInfo, Type parentType)
		{
			Type memberType;
			if (memberInfo is FieldInfo) {
				var fieldInfo = memberInfo as FieldInfo;
				memberType = fieldInfo.FieldType;
			} else {
				var propInfo = memberInfo as PropertyInfo;
				memberType = propInfo.PropertyType;
			}
			return new InjectableInfo (memberType);
		}
	}
}
