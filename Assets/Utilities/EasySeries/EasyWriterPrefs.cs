using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;

public partial class EasyWriter : IDisposable
{
	private string filePath;
	private static Dictionary<string, EasyData<string, EasyData>> files;

	public EasyData<string, EasyData> target {
		get {
			return files [filePath];
		}
	}

	public EasyWriter (string path)
	{
		filePath = path;
		if (files == null) {
			files = new Dictionary<string, EasyData<string, EasyData>> ();
			Observable.OnceApplicationQuit ().Subscribe (_ => {
				Dispose ();
			});
		}
		if (!files.ContainsKey (filePath))
			files.Add (filePath, Deserialize<EasyData<string, EasyData>> (filePath));
	}

	public void Dispose ()
	{
		Serialize<EasyData<string, EasyData>> (filePath, target);
	}

	public object GetObject (string key)
	{
		if (target.ToDictionary ().ContainsKey (key)) {
			return target.ToDictionary () [key].GetObject ();
		}
		return default (object);
	}

	public void SetObject (string key, object value)
	{
		if (target.ToDictionary ().ContainsKey (key)) {
			target.ToDictionary () [key] = new EasyData (value);
		} else {
			target.ToDictionary ().Add (key, new EasyData (value));
		}
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			Dispose ();
		}
		#endif
	}
}
