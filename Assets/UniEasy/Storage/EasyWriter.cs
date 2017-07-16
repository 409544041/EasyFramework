using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UniRx;

namespace UniEasy
{
	public partial class EasyWriter
	{
		private string filePath = "defaults";
		public static ReactiveDictionary<string, ReactiveWriter> Records;

		public EasyWriter (string path)
		{
			filePath = path;
			var directoryName = Path.GetDirectoryName (path);
			if (!Directory.Exists (directoryName)) {
				Directory.CreateDirectory (directoryName);
			}
			if (Records == null) {
				Records = new ReactiveDictionary<string, ReactiveWriter> ();
			}
			if (!Records.ContainsKey (path)) {
				DeserializeAsync<EasyDictionary<string, EasyObject>> (path).Subscribe (x => {
					Records.Add (path, new ReactiveWriter (path, x));
				});
			}
		}

		public IObservable<ReactiveWriter> OnAdd ()
		{
			return Records.ObserveAdd ()
				.StartWith (Records.Select (x => new DictionaryAddEvent<string, ReactiveWriter> (x.Key, x.Value)))
				.Where (x => x.Key.Equals (filePath)).Select (x => x.Value);
		}

		public IObservable<ReactiveWriter> OnRemove ()
		{
			return Records.ObserveRemove ().Where (x => x.Key.Equals (filePath)).Select (x => x.Value);
		}
	}
}
