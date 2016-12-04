using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;

public class EasyPrefs : IDisposable
{
	private string filePath;
	private static Dictionary<string, EasyData<string, EasyData>> files;

	public EasyData<string, EasyData> target {
		get {
			return files [filePath];
		}
	}

	public EasyPrefs (string path)
	{
		filePath = path;
		if (files == null) {
			files = new Dictionary<string, EasyData<string, EasyData>> ();
			Observable.OnceApplicationQuit ().Subscribe (_ => {
				Dispose ();
			});
		}
		if (!files.ContainsKey (filePath))
			files.Add (filePath, EasyWriter.Deserialize<EasyData<string, EasyData>> (filePath));
	}

	public void Dispose ()
	{
		EasyWriter.Serialize<EasyData<string, EasyData>> (filePath, target);
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
