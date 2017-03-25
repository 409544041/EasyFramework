using System;

namespace UniEasy.Edit
{
	[AttributeUsage (AttributeTargets.Constructor | AttributeTargets.Method
	| AttributeTargets.Parameter | AttributeTargets.Property
	| AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class EasyMenuItemAttribute : Attribute
	{
		public EasyMenuItemAttribute (string name)
		{
			Name = name;
		}

		public string Name {
			get;
			set;
		}
	}
}
