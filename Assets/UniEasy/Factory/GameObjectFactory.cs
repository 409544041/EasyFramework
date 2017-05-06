using System.Collections.Generic;
using UnityEngine;

namespace UniEasy
{
	[AddComponentMenu ("Factory/GameObject Factory")]
	public class GameObjectFactory : MonoBehaviour
	{
		static private GameObjectFactory instance;

		static public GameObjectFactory Instance {
			get {
				return instance;
			}
		}

		static public bool IsInitialised {
			get {
				bool b = instance != null ? true : false;
				return b;
			}
		}

		private Dictionary<string, Queue<GameObject>> pool;
		private Dictionary<string, List<GameObject>> actives;

		void Awake ()
		{
			instance = this;
			pool = new Dictionary<string, Queue<GameObject>> ();
			actives = new Dictionary<string, List<GameObject>> ();
		}

		void Start ()
		{
		}

		public GameObject FindObject (string name)
		{
			if (pool.ContainsKey (name) && pool [name] != null) {
				if (pool [name].Count > 0)
					return pool [name].Dequeue ();
			}
			return null;
		}

		public void AddObject (GameObject go)
		{
			if (go != null) {
				string name = go.name;
				if (pool.ContainsKey (name)) {
					if (pool [name] == null)
						pool [name] = new Queue<GameObject> ();
					if (!pool [name].Contains (go))
						pool [name].Enqueue (go);
				} else {
					Queue<GameObject> queue = new Queue<GameObject> ();
					queue.Enqueue (go);
					pool.Add (name, queue);
				}
			}
		}

		public GameObject CreateObject (GameObject go)
		{
			if (go != null) {
				string name = go.name;
				GameObject obj = FindObject (name);
				if (obj == null) {
					obj = Instantiate<GameObject> (go);
					obj.name = name;
				}
				return obj;
			}
			return null;
		}

		public GameObject CreateActiveObject (GameObject go)
		{
			GameObject obj = CreateObject (go);
			if (obj != null) {
				string name = obj.name;
				if (actives.ContainsKey (name)) {
					if (actives [name] == null) {
						actives [name] = new List<GameObject> ();
					}
					if (actives [name].Contains (obj)) {
						#if UNITY_EDITOR
						Debug.LogError ("can not create a already active object [" + obj + "]");
						#endif
					} else {
						actives [name].Add (obj);
					}
				} else {
					List<GameObject> list = new List<GameObject> () { obj };
					actives.Add (name, list);
				}
			}
			return obj;
		}

		public GameObject[] FindActiveObjects (string name)
		{
			if (actives.ContainsKey (name) && actives [name] != null) {
				return actives [name].ToArray ();
			}
			return null;
		}

		public void DisposeActiveObject (GameObject go)
		{
			if (go != null) {
				go.SetActive (false);
				string name = go.name;
				if (actives.ContainsKey (name) && actives [name] != null) {
					if (actives [name].Contains (go))
						actives [name].Remove (go);
				}
			}
			AddObject (go);
		}

		public void DisposeByName (string name)
		{
			GameObject[] items = FindActiveObjects (name);
			for (int i = 0; i < items.Length; i++) {
				Dispose (items [i]);
			}
		}

		public virtual void Dispose (GameObject go)
		{
			if (go != null) {
				go.transform.parent = null;
				DisposeActiveObject (go);
			}
		}
	}
}
