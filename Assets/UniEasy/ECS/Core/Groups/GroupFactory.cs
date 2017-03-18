using UniEasy.DI;
using System;

namespace UniEasy.ECS
{
	public class GroupFactory
	{
		[Inject] protected DiContainer Container = null;

		public Group Create (Type[] types)
		{
			var group = new Group (types);
			Container.Inject (group);
			return group;
		}
	}
}
