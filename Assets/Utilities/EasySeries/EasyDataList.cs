using System.Collections.Generic;
using System;

[Serializable] public class EasyBytes : EasyData<byte>
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

[Serializable] public class EasyBools : EasyData<bool>
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

[Serializable] public class EasyInts : EasyData<int>
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

[Serializable] public class EasyFloats : EasyData<float>
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

[Serializable] public class EasyStrings : EasyData<string>
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

[Serializable] public class EasyObjects : EasyData<object>
{
	public EasyObjects (List<object> value)
	{
		this.value = value;
	}

	public EasyObjects (object[] value)
	{
		this.value = new List<object> ();
		this.value.AddRange (value);
	}
}