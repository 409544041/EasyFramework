using System;

namespace UniEasy.Edit
{
	[AttributeUsage (AttributeTargets.Constructor | AttributeTargets.Method
	| AttributeTargets.Parameter | AttributeTargets.Property
	| AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class EasyMenuItem : Attribute
	{
		public string menuItem;
		public bool validate;
		public int priority;

		public EasyMenuItem (string itemName) : this (itemName, false)
		{
		}

		public EasyMenuItem (string itemName, bool isValidateFunction) : this (itemName, isValidateFunction, 0)
		{
		}

		public EasyMenuItem (string itemName, bool isValidateFunction, int priority)
		{
			this.menuItem = itemName;
			this.validate = isValidateFunction;
			this.priority = priority;
		}
	}
}
