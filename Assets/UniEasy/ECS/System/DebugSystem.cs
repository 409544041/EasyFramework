using UnityEngine;
using System.Linq;
using UniRx;
using System;

namespace UniEasy.ECS
{
	[Serializable]
	public class DebugLayer
	{
		public BoolReactiveProperty IsEnable = new BoolReactiveProperty (true);
		public StringReactiveProperty LayerName = new StringReactiveProperty ("");
	}

	public class DebugSystem : SystemBehaviour
	{
		[SerializeField]
		private DebugLayer[] DebugMask = new DebugLayer[0];

		protected override void Awake ()
		{
			base.Awake ();
		}

		void Start ()
		{
			Debugger.SetLayerMask (DebugMask.Where (layer => layer.IsEnable.Value)
				.Select (layer => layer.LayerName.Value).ToArray ());
		}
	}
}