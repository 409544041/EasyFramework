using UnityEngine;
using System.Collections.Generic;
using System;

namespace UniEasy
{
	[Serializable]
	public class EasyData
	{
		[SerializeField]
		private TypeCode typeCode;
		[SerializeField]
		private string value;

		public object GetObject ()
		{
			if (!string.IsNullOrEmpty (value)) {
				return Convert.ChangeType (value, typeCode);
			}
			return default (object);
		}

		public EasyData ()
		{
		}

		public EasyData (object value)
		{
			Type type = value.GetType ();
			if (type.IsSerializable) {
				this.value = Convert.ChangeType (value, typeof(string)).ToString ();
			}
			typeCode = Type.GetTypeCode (type);
		}
	}

	[Serializable]
	public class EasyData<T>
	{
		[SerializeField]
		protected List<T> value;

		public List<T> ToList ()
		{
			return this.value;
		}

		public EasyData ()
		{
		}

		public EasyData (List<T> value)
		{
			this.value = value;
		}

		public EasyData (T[] value)
		{
			this.value = new List<T> ();
			this.value.AddRange (value);
		}
	}

	[Serializable]
	public partial class EasyData<TKey, TValue> : ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<TKey> keys;
		[SerializeField]
		private List<TValue> values;
		private Dictionary<TKey, TValue> target;

		public EasyData (Dictionary<TKey, TValue> target)
		{
			this.target = target;
		}

		public void OnBeforeSerialize ()
		{
			keys = new List<TKey> (target.Keys);
			values = new List<TValue> (target.Values);
		}

		public void OnAfterDeserialize ()
		{
			var count = Math.Min (keys.Count, values.Count);
			target = new Dictionary<TKey, TValue> (count);
			for (var i = 0; i < count; ++i) {
				target.Add (keys [i], values [i]);
			}
		}

		public Dictionary<TKey, TValue> ToDictionary ()
		{
			return target;
		}
	}
}
