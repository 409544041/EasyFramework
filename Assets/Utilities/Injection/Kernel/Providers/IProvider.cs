using System.Collections.Generic;
using System;

namespace UniEasy
{
	public interface IProvider
	{
		Type GetInstanceType (InjectContext context);

		IEnumerator<List<object>> GetAllInstancesWithInjectSplit (InjectContext context);
	}
}
