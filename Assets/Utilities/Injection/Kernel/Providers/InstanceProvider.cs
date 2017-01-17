using System;

namespace UniEasy
{
	public class InstanceProvider : IProvider
	{
		private Type instanceType;
		private object instance;

		public InstanceProvider (Type instanceType, object instance)
		{
			this.instanceType = instanceType;
			this.instance = instance;
		}

		public Type GetInstanceType ()
		{
			return instanceType;
		}
	}
}
