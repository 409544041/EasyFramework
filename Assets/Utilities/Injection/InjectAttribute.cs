using System;

namespace UniEasy
{
	[AttributeUsage (AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class InjectAttribute : Attribute
	{
		public InjectAttribute ()
		{
		}
	}
}
