using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;

namespace UniEasy.ECS
{
	public static class GroupExtensions
	{
		static Dictionary<Group, ReactiveCollection<IEntity>> ActivePool = new Dictionary<Group, ReactiveCollection<IEntity>> ();
		static Dictionary<Group, ReactiveCollection<IEntity>> NonactivePool = new Dictionary<Group, ReactiveCollection<IEntity>> ();

		public static ReactiveCollection<IEntity> GetEntities (this Group group, bool isActive)
		{
			if (!ActivePool.ContainsKey (group)) {
				ActivePool.Add (group, new ReactiveCollection<IEntity> ());
			}
			if (!NonactivePool.ContainsKey (group)) {
				NonactivePool.Add (group, new ReactiveCollection<IEntity> ());
			}
			var active = ActivePool [group];
			var nonactive = NonactivePool [group];
			group.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
				var entityBehaviour = entity.GetComponent<EntityBehaviour> ();
				if (entityBehaviour.isActiveAndEnabled) {
					if (!active.Contains (entity))
						active.Add (entity);
					if (nonactive.Contains (entity))
						nonactive.Remove (entity);
				} else if (!entityBehaviour.isActiveAndEnabled && active.Contains (entity)) {
					if (active.Contains (entity))
						active.Remove (entity);
					if (!nonactive.Contains (entity))
						nonactive.Add (entity);
				}
				entityBehaviour.gameObject.OnEnableAsObservable ().Subscribe (_ => {
					if (!active.Contains (entity))
						active.Add (entity);
					if (nonactive.Contains (entity))
						nonactive.Remove (entity);
				}).AddTo (group.Disposer).AddTo (entityBehaviour.Disposer);
				entityBehaviour.gameObject.OnDisableAsObservable ().Subscribe (_ => {
					if (active.Contains (entity))
						active.Remove (entity);
					if (!nonactive.Contains (entity))
						nonactive.Add (entity);
				}).AddTo (group.Disposer).AddTo (entityBehaviour.Disposer);
			}).AddTo (group.Disposer);
			return isActive == true ? ActivePool [group] : NonactivePool [group];
		}
	}
}
