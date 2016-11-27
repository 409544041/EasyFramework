using System.Collections.Generic;
using System;

public partial class EasyBasic
{
	
}

[Serializable] public class EasyByte : EasyBasic<byte>
{
	public EasyByte (List<byte> value)
	{
		this.value = value;
	}

	public EasyByte (byte[] value)
	{
		this.value = new List<byte> ();
		this.value.AddRange (value);
	}
}

[Serializable] public class EasyBool : EasyBasic<bool>
{
	public EasyBool (List<bool> value)
	{
		this.value = value;
	}

	public EasyBool (bool[] value)
	{
		this.value = new List<bool> ();
		this.value.AddRange (value);
	}
}

[Serializable] public class EasyInt : EasyBasic<int>
{
	public EasyInt (List<int> value)
	{
		this.value = value;
	}

	public EasyInt (int[] value)
	{
		this.value = new List<int> ();
		this.value.AddRange (value);
	}
}

[Serializable] public class EasyFloat : EasyBasic<float>
{
	public EasyFloat (List<float> value)
	{
		this.value = value;
	}

	public EasyFloat (float[] value)
	{
		this.value = new List<float> ();
		this.value.AddRange (value);
	}
}

[Serializable] public class EasyString : EasyBasic<string>
{
	public EasyString (List<string> value)
	{
		this.value = value;
	}

	public EasyString (string[] value)
	{
		this.value = new List<string> ();
		this.value.AddRange (value);
	}
}