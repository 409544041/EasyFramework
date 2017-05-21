using UnityEngine;
using UniEasy.DI;
using System;
using UniRx;

namespace UniEasy.ECS
{
	public class SystemBehaviour : MonoBehaviour, ISystem, IDisposable, IDisposableContainer
	{
		[Inject]
		public IEventSystem EventSystem { get; set; }

		[Inject]
		public IPoolManager PoolManager { get; set; }

		[Inject]
		protected GroupFactory GroupFactory { get; set; }

		[Inject]
		protected PrefabFactory PrefabFactory { get; set; }

		private CompositeDisposable disposer = new CompositeDisposable ();

		public CompositeDisposable Disposer {
			get { return disposer; }
			set { disposer = value; }
		}

		[Inject]
		public virtual void Setup ()
		{

		}

		void Start ()
		{
	
		}

		public virtual void OnDestroy ()
		{
			Dispose ();
		}

		public virtual void Dispose ()
		{
			Disposer.Dispose ();
		}
	}
}
