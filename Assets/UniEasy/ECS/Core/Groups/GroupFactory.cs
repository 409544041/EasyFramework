using System.Collections.Generic;
using UniEasy.DI;
using System;

namespace UniEasy.ECS
{
	public class GroupFactory
	{
		[Inject] protected DiContainer Container = null;

		protected Dictionary<Type[], Group> GroupPool = new Dictionary<Type[], Group> ();

		public Group Create (params Type[] types)
		{
			var group = new Group (types);
			Container.Inject (group);
			return group;
		}

		public Group CreateAsSingle (params Type[] types)
		{
			if (GroupPool.ContainsKey (types)) {
				return GroupPool [types];
			}
			return Create (types);
		}
	}
}
