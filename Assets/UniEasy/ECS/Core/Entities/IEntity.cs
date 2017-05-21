using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniEasy.ECS
{
	public interface IEntity
	{
		int Id { get; }

		IEnumerable<object> Components { get; }

		void AddComponent (object component);

		void AddComponent<T> () where T : class, new();

		void RemoveComponent (object component);

		void RemoveComponent<T> () where T : class;

		T GetComponent<T> () where T : class;

		bool HasComponent<T> () where T : class;

		bool HasComponents (params Type[] component);
	}
}
