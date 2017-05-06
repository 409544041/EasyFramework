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

		public override void Setup ()
		{
			base.Setup ();

			var components = GetComponents<Component> ();
			for (int i = 0; i < components.Length; i++) {
				if (components [i] == null) {
					Debug.LogWarning ("Component on " + this.gameObject.name + " is missing!");
				} else {
					var type = components [i].GetType ();
					if (type != typeof(Transform)) {
						if (!Entity.HasComponents (type)) {
							Entity.AddComponent (components [i]);
						} else {
							Debug.LogError ("Cannot add multiple identical components on " + this.gameObject.name + " this is not supported!");
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
