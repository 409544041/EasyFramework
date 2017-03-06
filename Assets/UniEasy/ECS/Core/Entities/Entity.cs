using System.Collections.Generic;
using System.Linq;
using System;

namespace UniEasy.ECS
{
	public class Entity : IEntity
	{
		private readonly Dictionary<Type, object> components;

		public int Id { get; private set; }

		public IEnumerable<object> Components { get { return components.Values; } }

		public Entity (int id, IEventSystem eventSystem)
		{
			Id = id;
			EventSystem = eventSystem;
			components = new Dictionary<Type, object> ();
		}

		public void AddComponent (object component)
		{
			components.Add (component.GetType (), component);
		}

		public void AddComponent<T> () where T : class, new()
		{
			AddComponent (new T ());
		}

		public void RemoveComponent (object component)
		{
			if (!components.ContainsKey (component.GetType ())) {
				return;
			}

			if (component is IDisposable) {
				(component as IDisposable).Dispose ();
			}

			components.Remove (component.GetType ());
		}

		public void RemoveComponent<T> () where T : class
		{
			if (!HasComponent<T> ()) {
				return;
			}

			var component = GetComponent<T> ();
			RemoveComponent (component);
		}

		public bool HasComponent<T> () where T : class
		{
			return components.ContainsKey (typeof(T));
		}

		public bool HasComponents (params Type[] componentTypes)
		{
			if (components.Count == 0) {
				return false;
			}

			return componentTypes.All (x => components.ContainsKey (x));
		}

		public T GetComponent<T> () where T : class
		{
			return components [typeof(T)] as T;
		}
	}
}
