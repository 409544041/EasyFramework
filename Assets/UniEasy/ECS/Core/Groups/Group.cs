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

		ReactiveCollection<IEntity> entities = new ReactiveCollection<IEntity> ();

		public ReactiveCollection<IEntity> Entities {
			get { return entities; }
			set { entities = value; }
		}

		public IEnumerable<Type> Components { get; set; }

		public Predicate<IEntity> Predicate { get; set; }

		protected CompositeDisposable disposer = new CompositeDisposable ();

		public CompositeDisposable Disposer {
			get { return disposer; }
			set { disposer = value; }
		}

		public Group (params Type[] components)
		{
			Components = components;
			Predicate = null;
		}

		[Inject]
		public void Setup (IEventSystem eventSystem, IPoolManager poolManager)
		{
			EventSystem = eventSystem;
			EntityPool = poolManager.GetPool ();

			var entities = EntityPool.Entities.ToArray ();
			for (int i = 0; i < entities.Length; i++) {
				if (entities [i].HasComponents (Components.ToArray ())) {
					AddEntity (entities [i]);
				}
			}

			EventSystem.Receive<EntityAddedEvent> ().Where (x => x.Entity.HasComponents (Components.ToArray ())).Subscribe (x => {
				AddEntity (x.Entity);
			}).AddTo (this);

			EventSystem.Receive<EntityRemovedEvent> ().Where (x => Entities.Contains (x.Entity)).Subscribe (x => {
				RemoveEntity (x.Entity);
			}).AddTo (this);

			EventSystem.Receive<ComponentAddedEvent> ().Where (x => x.Entity.HasComponents (Components.ToArray ()) && Entities.Contains (x.Entity) == false).Subscribe (x => {
				AddEntity (x.Entity);
			}).AddTo (this);

			EventSystem.Receive<ComponentRemovedEvent> ().Where (x => Components.Contains (x.Component.GetType ()) && Entities.Contains (x.Entity)).Subscribe (x => {
				RemoveEntity (x.Entity);
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
	}
}
