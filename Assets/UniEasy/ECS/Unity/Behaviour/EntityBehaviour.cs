using UnityEngine;

namespace UniEasy.ECS
{
	public class EntityBehaviour : ComponentBehaviour
	{
		public IPool Pool {
			get {
				if (pool != null) {
					return pool;
				} else {
					return (pool = PoolManager.GetPool ());
				}
			}
			set { pool = value; }
		}

		private IPool pool;

		public IEntity Entity {
			get {
				return entity == null ? (entity = Pool.CreateEntity ()) : entity;
			}
			set {
				entity = value;
			}
		}

		private IEntity entity;

		protected override void Awake ()
		{
			base.Awake ();

			var components = GetComponents<Component> ();
			for (int i = 0; i < components.Length; i++) {
				if (components [i] == null) {
					Debug.LogWarning ("Component on " + this.gameObject.name + " is missing!");
				} else {
					var type = components [i].GetType ();
					if (type != typeof(Transform)) {
						if (type == typeof(EntityBehaviour)) {
							if (!Entity.HasComponent<EntityBehaviour> ())
								Entity.AddComponent (components [i]);
						} else {
							Entity.AddComponent (components [i]);
						}
					}
				}
			}
		}

		public override void OnDestroy ()
		{
			Pool.RemoveEntity (Entity);

			base.OnDestroy ();
		}
	}
}
