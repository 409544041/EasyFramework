using System.Collections.Generic;
using System;

namespace UniEasy
{
	public class CachedProvider : IProvider
	{
		readonly IProvider creator;

		List<object> instances;

		public CachedProvider (IProvider creator)
		{
			this.creator = creator;
		}

		public Type GetInstanceType ()
		{
			return creator.GetInstanceType ();
		}

		public IEnumerator<List<object>> GetAllInstancesWithInjectSplit ()
		{
			if (instances != null) {
				yield return instances;
				yield break;
			}

			var runner = creator.GetAllInstancesWithInjectSplit ();

			// First get instance
			bool hasMore = runner.MoveNext ();

			instances = runner.Current;

			yield return instances;

			// Now do injection
			while (hasMore) {
				hasMore = runner.MoveNext ();
			}
		}
	}
}
