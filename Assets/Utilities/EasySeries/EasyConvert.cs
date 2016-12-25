using UnityEngine;
using System.Collections.Generic;
using System;

public class EasyConvert
{
	static public string ArrayToString<T> (object value)
	{
		Type type = typeof(T);
		T[] array = default (T[]);
		if (value.GetType () == typeof(List<T>)) {
			array = (value as List<T>).ToArray ();
		} else if (value.GetType () == typeof(T[])) {
			array = value as T[];
		} else {
			Debug.LogError ("Sorry, this function only support T[] or List<T>!");
			return default (string);
		}
		if (type == typeof(string)) {
			return JsonUtility.ToJson (new EasyStrings (array as string[]));
		} else if (type.IsArray) {
		} else if (type.IsSerializable && type.IsPrimitive) {
			EasyData[] datas = new EasyData[array.Length];
			for (int i = 0; i < datas.Length; i++) {
				datas [i] = new EasyData (array [i]);
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

	static public T[] StringToArray<T> (string value)
	{
		Type type = typeof(T);
		T[] array = default (T[]);
		if (type == typeof(string)) {
			array = JsonUtility.FromJson<EasyStrings> (value).ToList ().ToArray () as T[];
		} else if (type.IsArray) {
		} else if (type.IsSerializable && type.IsPrimitive) {
			EasyData[] datas = JsonUtility.FromJson<EasyObjects> (value).ToList ().ToArray ();
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
