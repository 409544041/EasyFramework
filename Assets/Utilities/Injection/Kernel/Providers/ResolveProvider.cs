using System.Collections.Generic;
using System.Linq;
using System;

namespace UniEasy
{
	public class ResolveProvider : IProvider
	{
		readonly object identifier;
		readonly DiContainer container;
		readonly Type contractType;

		public ResolveProvider (Type contractType, DiContainer container, object identifier)
		{
			this.contractType = contractType;
			this.identifier = identifier;
			this.container = container;
		}

		public Type GetInstanceType ()
		{
			return contractType;
		}

		public IEnumerator<List<object>> GetAllInstancesWithInjectSplit ()
		{
			var context = new InjectContext (container, contractType, identifier);
			yield return container.ResolveAll (context).Cast<object> ().ToList ();
		}
	}
}
