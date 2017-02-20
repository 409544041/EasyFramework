using System.Collections.Generic;
using System;
using System.Linq;

namespace UniEasy.DI
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

		public Type GetInstanceType (InjectContext context)
		{
			return instanceType;
		}

		public IEnumerator<List<object>> GetAllInstancesWithInjectSplit (InjectContext context)
		{
			yield return new List<object> () { instance };
		}
	}
}
