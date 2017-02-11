using System.Collections.Generic;
using System;
using System.Linq;

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

		public IEnumerator<List<object>> GetAllInstancesWithInjectSplit ()
		{
			yield return new List<object>() { instance };
		}
	}
}
