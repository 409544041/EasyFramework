using System.Collections.Generic;
using UniEasy.DI;
using System;
using UniRx;

namespace UniEasy.ECS
{
	public class GroupFactory
	{
		[Inject] protected DiContainer Container = null;
		private Type[] types;
		private List<Func<IEntity, ReactiveProperty<bool>>> predicates = new List<Func<IEntity, ReactiveProperty<bool>>> ();

		public Group Create (params Type[] types)
		{
			this.types = types;
			return this.Create ();
		}

		public Group Create ()
		{
			var group = new Group (types, predicates);
			Container.Inject (group);

			types = null;
			predicates.Clear ();
			return group;
		}

		public GroupFactory AddTypes (params Type[] types)
		{
			this.types = types;
			return this;
		}

		public GroupFactory WithPredicate (Func<IEntity, ReactiveProperty<bool>> predicate)
		{
			this.predicates.Add (predicate);
			return this;
		}
	}
}
