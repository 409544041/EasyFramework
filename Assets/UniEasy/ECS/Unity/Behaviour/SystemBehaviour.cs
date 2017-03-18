using UnityEngine;
using UniEasy.DI;
using System;
using UniRx;

namespace UniEasy.ECS
{
	public class SystemBehaviour : MonoBehaviour, ISystem, IBehaviour, IDisposable
	{
		[Inject]
		public IEventSystem EventSystem { get; set; }

		[Inject]
		public IPoolManager PoolManager { get; set; }

		[Inject]
		protected GroupFactory GroupFactory { get; set; }

		private CompositeDisposable disposer = new CompositeDisposable ();

		public CompositeDisposable Disposer {
			get { return disposer; }
			set { disposer = value; }
		}

		protected virtual void Awake ()
		{
			this.InjectSelf ();
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
