using System.Collections.Generic;
using UnityEngine;

namespace UniEasy
{
	[AddComponentMenu ("Factory/Block Factory")]
	public class BlockFactory : MonoBehaviour
	{
		static private BlockFactory instance;

		static public BlockFactory Instance {
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

		private Dictionary<string, EasyBlock> target;
		private Dictionary<string, List<string>> snapshots;
		private Queue<GameObject> actives;

		void Awake ()
		{
			instance = this;
			target = new Dictionary<string, EasyBlock> ();
			snapshots = new Dictionary<string, List<string>> ();
			actives = new Queue<GameObject> ();
		}

		void Start ()
		{
	
		}

		public void AddBlock (EasyBlock asset, string name = "")
		{
			if (string.IsNullOrEmpty (name)) {
				name = asset.name;
			}
			if (!string.IsNullOrEmpty (name) && asset != null) {
				if (target.ContainsKey (name)) {
					#if UNITY_EDITOR
					Debug.LogError ("Sorry, add block failed!\nbecause block name [" + name + "] conflict.");
					#endif
				} else {
					target.Add (name, asset);
				}
			}
		}

		public void AddBlockRange (EasyBlock[] assets)
		{
			for (int i = 0; i < assets.Length; i++) {
				AddBlock (assets [i], assets [i].name);
			}
		}

		public GameObject CreateBlock (string name)
		{
			if (target != null && target.ContainsKey (name)) {
				GameObject go = new GameObject (name);
				var items = target [name].ToDictionary ().GetEnumerator ();
				while (items.MoveNext ()) {
					BlockObject block = (BlockObject)items.Current.Value;
					GameObject item = GameObjectFactory.Instance.CreateActiveObject (block.gameObject);
					item.transform.parent = go.transform;
					item.transform.localPosition = block.localPosition;
					item.transform.localRotation = block.localRotation;
					item.transform.localScale = block.localScale;
					item.SetActive (true);
				}
				actives.Enqueue (go);
				return go;
			}
			return null;
		}

		public void CreateSnapShot (string name, List<string> target)
		{
			if (target == null) {
				return;
			}
			if (snapshots.ContainsKey (name)) {
				#if UNITY_EDITOR
				Debug.LogError ("Can not create snapshot because already have same name [" + name + "]");
				#endif
			} else {
				snapshots.Add (name, target);
			}
		}

		public GameObject CreateBlockBySnapShots (string name)
		{
			if (snapshots != null && snapshots.ContainsKey (name)) {
				List<string> list = snapshots [name];
				if (list != null && list.Count > 0) {
					int index = Random.Range (0, list.Count);
					return CreateBlock (list [index]);
				}
			}
			return null;
		}

		public void Dispose (GameObject block)
		{
			if (block == null)
				return;
			for (int i = 0; i < block.transform.childCount; i++) {
				GameObject go = block.transform.GetChild (i).gameObject;
				GameObjectFactory.Instance.Dispose (go);
			}
			Destroy (block);
		}

		public void Dispose ()
		{
			if (actives != null && actives.Count > 0) {
				Dispose (actives.Dequeue ());
			}
		}
	}
}
