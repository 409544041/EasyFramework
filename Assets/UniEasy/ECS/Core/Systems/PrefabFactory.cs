using UnityEngine;
using UniEasy.DI;

namespace UniEasy.ECS
{
	public class PrefabFactory
	{
		[Inject]
		protected DiContainer Container = null;

		public T Instantiate<T> (T original, Transform parent = null, bool worldPositionStays = false) where T : UnityEngine.Object
		{
			return GameObject.Instantiate<T> (original, parent, worldPositionStays);
		}

		public GameObject Instantiate (GameObject prefab, Transform parent = null, bool worldPositionStays = false)
		{
			var go = Instantiate<GameObject> (prefab, parent, worldPositionStays);
			var entityBehaviour = go.GetComponent<EntityBehaviour> () ?? go.AddComponent<EntityBehaviour> ();
			Container.Inject (entityBehaviour);
			return go;
		}
	}
}
