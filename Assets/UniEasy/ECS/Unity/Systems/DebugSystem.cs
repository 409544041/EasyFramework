using UnityEngine;
using System.Linq;
using UniRx;
using System;

namespace UniEasy.ECS
{
	[Serializable]
	public struct DebugLayer
	{
		public bool IsEnable;
		public string LayerName;
	}

	[Serializable]
	public class DebugMask : EasyList<DebugLayer>
	{
	}

	public class DebugSystem : SystemBehaviour
	{
		[SerializeField]
		private DebugMask DebugMask;
		private EasyWriter DebugWriter;

		protected override void Awake ()
		{
			base.Awake ();
		}

		void Start ()
		{
			DebugWriter = new EasyWriter (Application.streamingAssetsPath + "/DebugSystem/DefaultConfig.json");

//			if (!DebugWriter.HasKey ("Default")) {
//				DebugMask = ScriptableObject.CreateInstance<DebuggerAsset> ();
//				DebugMask = ScriptableObjectFactory.CreateAsset<DebuggerAsset> (DebugMask,
//					Application.streamingAssetsPath + "/DebugSystem/DebugMask.asset");
//				DebugWriter.Set ("Default", DebugMask);
//			} else {
//				DebugMask = ScriptableObjectFactory.LoadAssetAtPath<DebuggerAsset> (
//					Application.streamingAssetsPath + "/DebugSystem/DebugMask.asset");
//				DebugWriter.Get ("Default", DebugMask);
//			}
		}
	}
}