using UnityEngine;
using System.Collections.Generic;
using System;

namespace UniEasy
{
	public static class ConvertExtensions
	{
		static public string ToString<T> (this object value)
		{
			if (value.GetType () == typeof(List<T>)) {
				return (value as List<T>).ToArray ().ToString<T> ();
			} else if (value.GetType () == typeof(T[])) {
				return (value as T[]).ToString<T> ();
			}
			return "";
		}

		static public string ToString<T> (this T[] array)
		{
			Type type = typeof(T);
			if (type == typeof(string)) {
				return JsonUtility.ToJson (new EasyStrings (array as string[]));
			} else if (type.IsArray) {
			} else if (type.IsSerializable && type.IsPrimitive) {
				EasyObject[] datas = new EasyObject[array.Length];
				for (int i = 0; i < datas.Length; i++) {
					datas [i] = new EasyObject (array [i]);
				}
				return JsonUtility.ToJson (new EasyObjects (datas));
			} else if (type.IsSerializable && type.IsEnum) {
			} else if (type.IsSerializable && type == typeof(Nullable)) {
			} else if (type.IsSerializable && (type.IsClass || type.IsValueType)) {
				string[] content = new string[array.Length];
				for (int i = 0; i < content.Length; i++) {
					content [i] = JsonUtility.ToJson (array [i]);
				}
				return JsonUtility.ToJson (new EasyStrings (content));
			}
			return default (string);
		}

		static public T[] ToArray<T> (this string value)
		{
			Type type = typeof(T);
			T[] array = default (T[]);
			if (type == typeof(string)) {
				array = JsonUtility.FromJson<EasyStrings> (value).ToList ().ToArray () as T[];
			} else if (type.IsArray) {
			} else if (type.IsSerializable && type.IsPrimitive) {
				EasyObject[] datas = JsonUtility.FromJson<EasyObjects> (value).ToList ().ToArray ();
				array = new T[datas.Length];
				for (int i = 0; i < datas.Length; i++) {
					array [i] = (T)datas [i].GetObject ();
				}
			} else if (type.IsSerializable && type.IsEnum) {
			} else if (type.IsSerializable && type == typeof(Nullable)) {
			} else if (type.IsSerializable && (type.IsClass || type.IsValueType)) {
				string[] datas = JsonUtility.FromJson<EasyStrings> (value).ToList ().ToArray ();
				array = new T[datas.Length];
				for (int i = 0; i < datas.Length; i++) {
					array [i] = JsonUtility.FromJson<T> (datas [i]);
				}
			}
			return array;
		}
	}
}
