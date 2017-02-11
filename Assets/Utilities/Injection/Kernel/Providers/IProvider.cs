using System.Collections.Generic;
using System;

namespace UniEasy
{
	public interface IProvider
	{
		Type GetInstanceType();

		IEnumerator<List<object>> GetAllInstancesWithInjectSplit ();
	}
}
