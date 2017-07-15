using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniEasy
{
	[Serializable]
	public class EasyObject
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

		public EasyObject ()
		{
		}

		public EasyObject (object value)
		{
			Type type = value.GetType ();
			if (type.IsSerializable) {
				this.value = Convert.ChangeType (value, typeof(string)).ToString ();
			}
			typeCode = Type.GetTypeCode (type);
		}
	}

	[Serializable]
	public class EasyList<T>
	{
		[SerializeField]
		protected List<T> value;

		public List<T> ToList ()
		{
			return this.value;
		}

		public EasyList ()
		{
		}

		public EasyList (List<T> value)
		{
			this.value = value;
		}

		public EasyList (T[] value)
		{
			this.value = new List<T> ();
			this.value.AddRange (value);
		}
	}

	[Serializable]
	public partial class EasyDictionary<TKey, TValue> : ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<TKey> keys;
		[SerializeField]
		private List<TValue> values;
		private Dictionary<TKey, TValue> target;

		public EasyDictionary ()
		{
			this.target = new Dictionary<TKey, TValue> ();
		}

		public EasyDictionary (Dictionary<TKey, TValue> target)
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

	[Serializable] public class EasyBytes : EasyList<byte>
	{
		public EasyBytes (List<byte> value)
		{
			this.value = value;
		}

		public EasyBytes (byte[] value)
		{
			this.value = new List<byte> ();
			this.value.AddRange (value);
		}
	}

	[Serializable] public class EasyBools : EasyList<bool>
	{
		public EasyBools (List<bool> value)
		{
			this.value = value;
		}

		public EasyBools (bool[] value)
		{
			this.value = new List<bool> ();
			this.value.AddRange (value);
		}
	}

	[Serializable] public class EasyInts : EasyList<int>
	{
		public EasyInts (List<int> value)
		{
			this.value = value;
		}

		public EasyInts (int[] value)
		{
			this.value = new List<int> ();
			this.value.AddRange (value);
		}
	}

	[Serializable] public class EasyFloats : EasyList<float>
	{
		public EasyFloats (List<float> value)
		{
			this.value = value;
		}

		public EasyFloats (float[] value)
		{
			this.value = new List<float> ();
			this.value.AddRange (value);
		}
	}

	[Serializable] public class EasyStrings : EasyList<string>
	{
		public EasyStrings (List<string> value)
		{
			this.value = value;
		}

		public EasyStrings (string[] value)
		{
			this.value = new List<string> ();
			this.value.AddRange (value);
		}
	}

	[Serializable] public class EasyObjects : EasyList<EasyObject>
	{
		public EasyObjects (List<EasyObject> value)
		{
			this.value = value;
		}

		public EasyObjects (EasyObject[] value)
		{
			this.value = new List<EasyObject> ();
			this.value.AddRange (value);
		}
	}
}
