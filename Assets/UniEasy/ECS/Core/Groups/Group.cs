using System.Collections.Generic;
using System.Linq;
using UniEasy.DI;
using System;
using UniRx;

namespace UniEasy.ECS
{
	public class Group : IGroup, IDisposable, IDisposableContainer
	{
		public IEventSystem EventSystem { get; set; }

		public IPool EntityPool { get; set; }

		public string Name { get; set; }

		ReactiveCollection<IEntity> cachedEntities = new ReactiveCollection<IEntity> ();
		ReactiveCollection<IEntity> entities = new ReactiveCollection<IEntity> ();

		public ReactiveCollection<IEntity> Entities {
			get { return entities; }
			set { entities = value; }
		}

		public IEnumerable<Type> Components { get; set; }

		protected List<Func<IEntity, ReactiveProperty<bool>>> Predicates { get; set; }

		protected CompositeDisposable disposer = new CompositeDisposable ();

		public CompositeDisposable Disposer {
			get { return disposer; }
			set { disposer = value; }
		}

		public Group (Type[] components, List<Func<IEntity, ReactiveProperty<bool>>> predicates)
		{
			Components = components;
			Predicates = new List<Func<IEntity, ReactiveProperty<bool>>> ();
			for (int i = 0; i < predicates.Count; i++) {
				Predicates.Add (predicates [i]);
			}
		}

		[Inject]
		public void Setup (IEventSystem eventSystem, IPoolManager poolManager)
		{
			EventSystem = eventSystem;
			EntityPool = poolManager.GetPool ();

			cachedEntities.ObserveAdd ().Select (x => x.Value).Subscribe (entity => {
				if (Predicates.Count == 0) {
					PreAdd (entity);
					AddEntity (entity);
					return;
				}

				var bools = new List<ReactiveProperty<bool>> ();
				for (int i = 0; i < Predicates.Count; i++) {
					bools.Add (Predicates [i].Invoke (entity));
				}
				var onLatest = Observable.CombineLatest (bools.ToArray ());
				onLatest.DistinctUntilChanged ().Subscribe (values => {
					if (values.All (value => value == true)) {
						PreAdd (entity);
						AddEntity (entity);
					} else {
						PreRemove (entity);
						RemoveEntity (entity);
					}
				}).AddTo (this.Disposer);
			}).AddTo (this.Disposer);

			cachedEntities.ObserveRemove ().Select (x => x.Value).Subscribe (entity => {
				PreRemove (entity);
				RemoveEntity (entity);
			}).AddTo (this.Disposer);

			var entities = EntityPool.Entities.ToArray ();
			for (int i = 0; i < entities.Length; i++) {
				if (entities [i].HasComponents (Components.ToArray ())) {
					cachedEntities.Add (entities [i]);
				}
			}

			EventSystem.Receive<EntityAddedEvent> ().Where (x => x.Entity.HasComponents (Components.ToArray ()) && !cachedEntities.Contains (x.Entity)).Subscribe (x => {
				cachedEntities.Add (x.Entity);
			}).AddTo (this);

			EventSystem.Receive<EntityRemovedEvent> ().Where (x => cachedEntities.Contains (x.Entity)).Subscribe (x => {
				cachedEntities.Remove (x.Entity);
			}).AddTo (this);

			EventSystem.Receive<ComponentAddedEvent> ().Where (x => x.Entity.HasComponents (Components.ToArray ()) && !cachedEntities.Contains (x.Entity)).Subscribe (x => {
				cachedEntities.Add (x.Entity);
			}).AddTo (this);

			EventSystem.Receive<ComponentRemovedEvent> ().Where (x => Components.Contains (x.Component.GetType ()) && cachedEntities.Contains (x.Entity)).Subscribe (x => {
				cachedEntities.Remove (x.Entity);
			}).AddTo (this);
		}

		void AddEntity (IEntity entity)
		{
			Entities.Add (entity);
		}

		void RemoveEntity (IEntity entity)
		{
			Entities.Remove (entity);
		}

		public void Dispose ()
		{
			Disposer.Dispose ();
		}

		protected virtual void PreAdd (IEntity entity)
		{
		}

		protected virtual void PreRemove (IEntity entity)
		{
		}
	}
}
